from typing import List, TypedDict


class PubchemName(TypedDict):
    iupac_name: str
    synonyms: List[str]


class Prediction(TypedDict):
    """
    Represents a prediction.
    """

    product: str
    score: float
    reaction_image: str
    iupac_name: str
    synonyms: List[str]


class PredictionResult(TypedDict):
    """
    Represents the result of a prediction.
    """

    result: List[Prediction]
