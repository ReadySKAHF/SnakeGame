import io
import os
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

WORD1 = ''
WORD2 = ''


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


def parse_file(file_path):
    with open(file_path, "r", encoding="utf-8") as f:
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

            if "from-ip-text:" in line:
                current_data["from_ip"] = line.split("from-ip-text:")[-1][:12].strip()
                found = True

            elif "to-ip-text:" in line:
                current_data["to_ip"] = line.split("to-ip-text:")[-1][:12].strip()
                found = True

            elif "CALL-ID:" in line:
                current_data["call_id"] = line.split("CALL-ID:")[-1][:28].strip()
                found = True

            elif "from-mobile:" in line:
                current_data["from_number"] = line.split("from-mobile:")[-1].strip()
                found = True

            elif "to-mobile:" in line:
                current_data["to_number"] = line.split("to-mobile:")[-1].strip()
                found = True

            elif "calling-date:" in line:
                current_data["calling_date"] = to_date(line.split("calling-date:")[-1].strip())
                found = True

            elif "calling-time:" in line:
                current_data["calling_time"] = to_time(line.split("calling-time:")[-1].strip())
                found = True

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
