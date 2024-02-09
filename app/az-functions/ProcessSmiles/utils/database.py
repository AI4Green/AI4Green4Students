import os
from contextlib import contextmanager
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from dotenv import load_dotenv

load_dotenv()

# db connection string
connection_string = os.getenv(
    "AI4GREEN_DB_CONNECTION_STRING",
    "postgresql://postgres:example@localhost:5433/ai4green",
)
engine = create_engine(connection_string)

# session factory
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
