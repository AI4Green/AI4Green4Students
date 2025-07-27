from typing import TypedDict, List, Optional
from enum import Enum

class CompoundType(str, Enum):
    REACTANT = "Reactant"
    PRODUCT = "Product"
    
class Compound(TypedDict):
    id: int
    name: str
    molecular_weight: Optional[float]
    density: Optional[float]
    hazards: Optional[str]
    smiles: Optional[str]
    substance_type: CompoundType
    
class Metadata(TypedDict):
    number_of_reactants: int
    number_of_products: int

class ReactionTable(TypedDict):
    compounds: List[Compound]
    metadata: Metadata

