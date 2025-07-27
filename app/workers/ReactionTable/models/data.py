from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import declarative_base
from sqlalchemy import Column, Integer, Float, Text

# Create a base class using SQLAlchemy's declarative system
Base = declarative_base()


class Model(Base):
    """
    Base model class that can include CRUD convenience methods.
    """

    __abstract__ = True


class Compound(Model):
    __tablename__ = "Compound"

    id = Column(Integer, primary_key=True)
    cid = Column(Integer, unique=True)
    cas = Column(Text, nullable=False, unique=True)
    name = Column(Text, nullable=False)
    smiles = Column(Text)
    inchi = Column(Text)
    inchikey = Column(Text)
    molec_formula = Column(Text)
    density = Column(Float(53))
    concentration = Column(Float(53))
    boiling_point = Column(Float(53))
    melting_point = Column(Float(53))
    flash_point = Column(Float(53))
    autoignition_temp = Column(Float(53))
    molec_weight = Column(Float(53))
    state = Column(Text)
    form = Column(Text)
    hphrase = Column(Text)
    safety_score = Column(Float(53))
    health_score = Column(Float(53))
    enviro_score = Column(Float(53))
    econom_score = Column(Float(53))


class HazardCode(Model):
    __tablename__ = "HazardCode"

    id = Column(Integer, primary_key=True)
    code = Column(Text, nullable=False)
    phrase = Column(Text, nullable=False)
    category = Column(Text)
