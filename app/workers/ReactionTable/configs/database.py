import os
from contextlib import contextmanager
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker

# db connection string
connection_string = os.getenv(
    "DB_CONNECTION_STRING",
)

engine = create_engine(connection_string)

# session factory
SessionFactory = sessionmaker(bind=engine)


def _get_db_session():
    return SessionFactory()


@contextmanager
def db_session_scope():
    """Provide a transactional scope around a series of operations."""
    session = _get_db_session()
    try:
        yield session
    except Exception as e:
        # Log the exception if needed
        raise e
    finally:
        session.close()
