from sqlalchemy import Column, Integer, Text
from .base import Model


class HazardCode(Model):
    __tablename__ = "HazardCode"

    id = Column(Integer, primary_key=True)
    code = Column(Text, nullable=False)
    phrase = Column(Text, nullable=False)
    category = Column(Text)
