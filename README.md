import os
from datetime import datetime
import psycopg2

DIRECTORY = "example"

conn = psycopg2.connect(
    host="localhost",
    dbname="postgres",
    user="postgres",
    password="root",
)
cursor = conn.cursor()

for filename in os.listdir(DIRECTORY):
    if filename.endswith(".txt"):
        filepath = os.path.join(DIRECTORY, filename)

        created_date = datetime.fromtimestamp(os.path.getctime(filepath))

        with open(filepath, "r", encoding="utf-8") as f:
            for line in f:
                numberA, numberB, dateA, dateB, comment = line.strip().split(";")

                dateA = datetime.strptime(dateA, "%d-%m-%Y").date()
                dateB = datetime.strptime(dateB, "%d-%m-%Y:%H-%M")

                cursor.execute(
                    "INSERT INTO txt_files (created_date, numberA, numberB, dateA, dateB, comment) "
                    "VALUES (%s, %s, %s, %s, %s, %s)",
                    (created_date, numberA, numberB, dateA, dateB, comment),
                )

conn.commit()
conn.close()
print("Готово!")
