from typing import List
from pydantic import BaseModel


class PubChemName(BaseModel):
    iupac_name: str
    synonyms: List[str]


class PredictedProduct(BaseModel):
    """
    Represents a predicted product with their relative detail.
    """
    product: str
    score: float
    reaction_image: str
    iupac_name: str
    synonyms: List[str]


class PredictedProductResponse(BaseModel):
    """
    Represents the response structure for predicted products.
    """

    products: List[str]
    scores: List[float]
