from sqlalchemy import Column, Integer, Float, Text, ForeignKey
from sqlalchemy.orm import relationship

from .base import Model


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
