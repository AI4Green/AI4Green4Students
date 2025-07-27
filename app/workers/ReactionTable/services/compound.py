from typing import List
from rdkit import Chem
import logging

from ReactionTable.models.dto import Compound as CompoundDTO, CompoundType
from ReactionTable.models.data import Compound
from ReactionTable.configs.database import db_session_scope


class CompoundService:
    def list_by_smiles(
        self, smiles: List[str], type: CompoundType
    ) -> List[CompoundDTO]:
        """
        Process a batch of compounds efficiently.

        Args:
            smiles_list: List of SMILES strings
            compound_type: "reactant" or "product"

        Returns:
            List of compound data dictionaries
        """
        data: List[CompoundDTO] = []

        for i, smiles in enumerate(smiles):
            try:
                mol = Chem.MolFromSmiles(smiles)
                if mol is None:
                    raise f"Cannot process structure."

                inchi = Chem.MolToInchi(mol)
                compound = self._get_from_inchi(inchi)
                if compound is None:
                    raise f"Not found in database."

                compound_data = CompoundDTO(
                    id=compound.id,
                    name=compound.name if compound.name != "" else "Not found",
                    molecular_weight=(
                        compound.molec_weight if compound.molec_weight != "" else 0
                    ),
                    density=compound.density if compound.density != "" else "-",
                    hazards=(
                        compound.hphrase
                        if compound.hphrase != "No hazard codes found"
                        else "Unknown"
                    ),
                    smiles=smiles,
                    substance_type=type,
                )
                data.append(compound_data)
            except Exception as e:
                logging.error(f"Failed to process {type} {i + 1}: {e}")
                raise

        return data

    def _get_from_inchi(self, inchi: str) -> Compound:
        """
        Retrieves a compound by InChI

        Args:
            inchi: compound's InChI (structurally derived identifier)

        Returns:
            compound model.
        """
        with db_session_scope() as db:
            return db.query(Compound).filter(Compound.inchi == inchi).first()
