import sqlite3

# Step 1: Connect to SQLite database (creates the database file if it doesn't exist)
database_name = "demo_agent_data.db"
conn = sqlite3.connect(database_name)

# Step 2: Create a cursor object to interact with the database
cursor = conn.cursor()

# Step 3: Read SQL script from a .sql file and execute the commands
sql_file_path = "Chinook_Sqlite.sql"  # path to .sql file path
with open(sql_file_path, 'r', encoding='utf-8') as file:
    sql_script = file.read()

# Step 4: Execute the SQL script
cursor.executescript(sql_script)

# Step 5: Commit changes and close the connection
conn.commit()
conn.close()

print(f"Database '{database_name}' has been created and SQL script executed.")