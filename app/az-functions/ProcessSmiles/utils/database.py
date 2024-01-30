import os
from contextlib import contextmanager
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from dotenv import load_dotenv

load_dotenv()

# Set up the database connection string
connection_string = os.getenv("AI4GREEN_DB_CONNECTION_STRING")
engine = create_engine(connection_string)

# Create a session factory
SessionFactory = sessionmaker(bind=engine)


def get_db_session():
    return SessionFactory()


@contextmanager
def db_session_scope():
    """Provide a transactional scope around a series of operations."""
    session = get_db_session()
    try:
        yield session
    except Exception as e:
        # Log the exception if needed
        raise e
    finally:
        session.close()
