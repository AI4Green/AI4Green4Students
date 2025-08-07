from PIL import ImageDraw
from rdkit.Chem import Draw, rdChemReactions


def draw_reaction_image(reactants, product, save_path=None):
    rxn = rdChemReactions.ChemicalReaction()
    for mol in reactants:
        rxn.AddReactantTemplate(mol)
    rxn.AddProductTemplate(product)
    rxn.Initialize()

    img = Draw.ReactionToImage(rxn, subImgSize=(300, 300))

    if save_path:
        img.save(save_path)
    return img


def draw_labeled_reaction_image(reactants, product, label_text):
    img = draw_reaction_image(reactants, product)
    draw = ImageDraw.Draw(img)
    draw.text((10, 10), label_text, fill="black")  # Simple text overlay
    return img
