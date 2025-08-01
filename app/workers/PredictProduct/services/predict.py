from typing import List, Dict, Any
from PredictProduct.helpers.model_runner import ModelRunner
from PredictProduct.models.dto import PredictionResult, Prediction
from PredictProduct.helpers.smiles import canonicalize_smiles, smi_tokenizer, parse_smiles
from .pubchem_api import PubchemAPI
from rdkit import Chem
from PredictProduct.helpers.reaction_drawer import draw_labeled_reaction_image
from io import BytesIO
import base64


class PredictionService:
    def __init__(self, model: ModelRunner):
        self.model = model
        self.n_best = 30
        self.pubchem_api = PubchemAPI()

    def predict(self, smiles_list: List[str]) -> PredictionResult:
        """
        Takes a list of SMILES strings and returns the first N predictions (unsorted).
        """
        print("Starting prediction for SMILES:", smiles_list)

        tokenised = self._preprocess(smiles_list)
        results = self._inference(tokenised)

        first_n = self._get_top_n_predictions(results, n=5)[0]

        # Extract products and scores
        products = [entry["smiles"] for entry in first_n]
        scores = [entry["score"] for entry in first_n]

        reactants = parse_smiles(".".join(smiles_list))

        data: List[Prediction] = []

        for product, score in zip(products, scores):
            mol = Chem.MolFromSmiles(product)
            name = self.pubchem_api.get_name_from_pubchem(product)
            iupac = name["iupac_name"]
            synonyms = name["synonyms"]
            label = iupac if iupac else product
            img = draw_labeled_reaction_image(reactants, mol, label)
            buffer = BytesIO()
            img.save(buffer, format="PNG")
            image_b64 = base64.b64encode(buffer.getvalue()).decode("utf-8")
            data.append(
                Prediction(
                    product=product,
                    score=score,
                    reaction_image=image_b64,
                    iupac_name=iupac,
                    synonyms=synonyms,
                )
            )

        return PredictionResult(result=data)
    

    def _get_top_n_predictions(self,
        results: List[Dict[str, Any]], n: int = 5
    ) -> List[List[Dict[str, Any]]]:
        """
        Takes raw model results and returns the first N predictions (unsorted).
        """
        top_n_outputs = []

        for result in results:
            products = result["products"]
            scores = result["scores"]

            combined = list(zip(products, scores))[:n]  # No sorting, just take first n
            top_n = [{"smiles": p, "score": round(s, 2)} for p, s in combined]
            top_n_outputs.append(top_n)

        return top_n_outputs

    def _preprocess(self, smiles_list: List[str]) -> List[Dict[str, str]]:
        """
        Preprocess a list of SMILES strings into tokenized input for the model.
        """
        print(" Preprocessing SMILES:", smiles_list)
        tokenized_smiles = [
            {"src": smi_tokenizer(canonicalize_smiles(smi))} for smi in smiles_list
        ]
        print(" Tokenization result:", tokenized_smiles)
        return tokenized_smiles

    def _inference(self, tokenized_data: List[Dict[str, str]]) -> List[Dict[str, Any]]:
        """
        Run inference on tokenized SMILES using the model.
        """
        print(" Starting inference with tokenized input:", tokenized_data)

        results = []

        try:
            products, scores, _, _, _ = self.model.run(tokenized_data)
            print(" model.run() successful")
            assert (
                len(tokenized_data) * self.n_best == len(products) == len(scores)
            ), "Output size doesn't match expected n_best"

            for i in range(len(tokenized_data)):
                prod_subset = products[i * self.n_best : (i + 1) * self.n_best]
                score_subset = scores[i * self.n_best : (i + 1) * self.n_best]

                valid_products = []
                valid_scores = []
                for prod, score in zip(prod_subset, score_subset):
                    product = "".join(prod.split())
                    if not canonicalize_smiles(product):
                        continue
                    valid_products.append(product)
                    valid_scores.append(score)

                results.append({"products": valid_products, "scores": valid_scores})

        except Exception as e:
            print(" model.run() failed in batch mode:", str(e))
            for item in tokenized_data:
                try:
                    products, scores, *_ = self.model.run(inputs=[item])
                except Exception as inner_e:
                    print(" model.run() failed on single input:", str(inner_e))
                    products, scores = [], []

                valid_products = []
                valid_scores = []
                for prod, score in zip(products, scores):
                    product = "".join(prod.split())
                    if not canonicalize_smiles(product):
                        continue
                    valid_products.append(product)
                    valid_scores.append(score)

                results.append({"products": valid_products, "scores": valid_scores})

        print(" Inference results:", results)
        return results
