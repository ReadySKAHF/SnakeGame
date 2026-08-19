import io
import os
import re
import datetime
import psycopg

ROOT_DIR = r''

DB_CONFIG = {
    "host": "localhost",
    "port": 5432,
    "dbname": "postgres",
    "user": "postgres",
    "password": "root",
}

WORD1 = 'Via: SIP'
WORD2 = 'Content-Length: 0'


def to_date(value):
    try:
        return datetime.datetime.strptime(value, "%d.%m.%Y").date()
    except (ValueError, TypeError):
        return None


def to_time(value):
    try:
        return datetime.datetime.strptime(value, "%H:%M:%S").time()
    except (ValueError, TypeError):
        return None


SIP_RE = re.compile(r"sip:\s*\+?(?P<number>\d+)@(?P<ip>[^>]+)>", re.IGNORECASE)
HEADER_DATETIME_RE = re.compile(r"(\d{2}\.\d{2}\.\d{2})(\d{2}:\d{2}:\d{2})")


def parse_sip(line, prefix):
    part = line.split(prefix, 1)[-1]
    m = SIP_RE.search(part)
    if not m:
        return None, None
    return m.group("number"), m.group("ip").strip()


def expand_year(raw_date):
    day, month, year = raw_date.split(".")
    if len(year) == 2:
        year = "20" + year
    return f"{day}.{month}.{year}"


def read_calling_datetime(f):
    f.seek(32)
    raw = f.read(16)
    m = HEADER_DATETIME_RE.search(raw)
    if not m:
        return None, None
    return to_date(expand_year(m.group(1))), to_time(m.group(2))


def parse_file(file_path):
    with open(file_path, "r", encoding="utf-8") as f:
        calling_date, calling_time = read_calling_datetime(f)
        f.seek(0)
        content = f.read()

    parts = content.split(WORD1)
    blocks_data = []

    for part in parts[1:]:
        if WORD2 not in part:
            continue

        block_text = part.split(WORD2)[0].strip()
        block_file = io.StringIO(block_text)

        current_data = {
            "from_ip": None,
            "to_ip": None,
            "call_id": None,
            "from_number": None,
            "to_number": None,
            "calling_date": None,
            "calling_time": None,
        }
        found = False

        for line in block_file:
            line = line.strip()

            if line.startswith("From:"):
                current_data["from_number"], current_data["from_ip"] = parse_sip(line, "From:")
                found = True

            elif line.startswith("To:"):
                current_data["to_number"], current_data["to_ip"] = parse_sip(line, "To:")
                found = True

            elif "from-ip-text:" in line:
                current_data["from_ip"] = line.split("from-ip-text:")[-1][:12].strip()
                found = True

            elif "to-ip-text:" in line:
                current_data["to_ip"] = line.split("to-ip-text:")[-1][:12].strip()
                found = True

            elif "CALL-ID:" in line:
                current_data["call_id"] = line.split("CALL-ID:")[-1].split("@")[0].strip()
                found = True

            elif "from-mobile:" in line:
                if not current_data["from_number"]:
                    current_data["from_number"] = line.split("from-mobile:")[-1].strip()
                found = True

            elif "to-mobile:" in line:
                if not current_data["to_number"]:
                    current_data["to_number"] = line.split("to-mobile:")[-1].strip()
                found = True

        current_data["calling_date"] = calling_date
        current_data["calling_time"] = calling_time

        if found:
            blocks_data.append(current_data)

    return blocks_data


def collect_all_data(root_dir):
    all_data = []

    for dirpath, _, filenames in os.walk(root_dir):
        for filename in filenames:
            if not filename.lower().endswith(".txt"):
                continue

            file_path = os.path.join(dirpath, filename)

            try:
                blocks = parse_file(file_path)
            except (OSError, UnicodeDecodeError) as e:
                print(f'Ошибка чтения файла {file_path}: {e}')
                continue

            all_data.extend(blocks)

    return all_data


INSERT_QUERY = """
    INSERT INTO calls (from_ip, to_ip, call_id, from_number, to_number, calling_date, calling_time)
    VALUES (%(from_ip)s, %(to_ip)s, %(call_id)s, %(from_number)s, %(to_number)s, %(calling_date)s, %(calling_time)s)
"""


def save_to_db(records):
    if not records:
        print('Нет данных для сохранения.')
        return

    with psycopg.connect(**DB_CONFIG) as conn:
        with conn.cursor() as cur:
            cur.executemany(INSERT_QUERY, records)
        conn.commit()

    print(f'Сохранено записей: {len(records)}')


if __name__ == "__main__":
    data = collect_all_data(ROOT_DIR)

    for i, block in enumerate(data, 1):
        print(f'Блок {i}: {block}')

    print(f'Общее кол-во блоков: {len(data)}')

    save_to_db(data)
