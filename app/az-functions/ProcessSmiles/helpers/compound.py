from ProcessSmiles import models
from ProcessSmiles.utils.database import db_session_scope


def get_compound_from_inchi(inchi: str) -> models.Compound:
    """
    Retrieves a compound by InChI

    Args:
        inchi: compound's InChI (structurally derived identifier)

    Returns:
        compound model.
    """
    with db_session_scope() as db:
        return db.query(models.Compound).filter(models.Compound.inchi == inchi).first()
