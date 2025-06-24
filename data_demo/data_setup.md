# Installation
* this is specific to enabling chatting with sql data as a demo
* Determine if you need sqlite3 installed
```bash
 import sqlite3
print(sqlite3.sqlite_version)
```
* Ensure you have sqlparse installed
```bash
pip install sqlparse
```
* You should get an output like `3.45.1`
* If you need to install sqlite3, run the following command
```bash
pip install sqlite3
```

# Setup
* If you need to create a demo database and data, run the following script in the terminal, from the data_demo directory
* `python create_initial_db.py`
