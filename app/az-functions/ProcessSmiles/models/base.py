# coding: utf-8
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import declarative_base

# Create a base class using SQLAlchemy's declarative system
Base = declarative_base()


class Model(Base):
    """
    Base model class that can include CRUD convenience methods.
    """

    __abstract__ = True
