from flask import Flask, request, jsonify
import sqlite3

app = Flask(__name__)
DATABASE = 'high_scores.db'

def create_connection():
    """Create a database connection to the SQLite database."""
    conn = sqlite3.connect(DATABASE)
    return conn

def init_db():
    """Create the high_scores table if it doesn't exist."""
    create_table_query = ''' CREATE TABLE IF NOT EXISTS high_scores (
                                id INTEGER PRIMARY KEY,
                                connectionId TEXT NOT NULL,
                                score INTEGER NOT NULL
                            ); '''
    conn = create_connection()
    with conn:
        conn.execute(create_table_query)

@app.route('/add_score', methods=['POST'])
def add_score():
    data = request.get_json()  # corrected this line
    connectionId = data['connectionId']
    score = data['score']

    insert_query = ''' INSERT INTO high_scores(connectionId, score)
                       VALUES(?, ?) '''
    conn = create_connection()
    with conn:
        conn.execute(insert_query, (connectionId, score))
    return jsonify({"message": "Score added successfully"}), 200

if __name__ == '__main__':
    init_db()
    app.run(debug=True, host='0.0.0.0')
