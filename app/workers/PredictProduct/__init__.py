import json
import logging

import azure.functions as func

from .services.prediction import PredictionService


def main(req: func.HttpRequest) -> func.HttpResponse:
    smiles = req.params.get("smiles")
    if not smiles:
        logging.error("Missing required parameter")
        return _error_response("Missing required parameter")

    service = PredictionService()
    smiles = smiles.split(",")[0]
    try:
        prediction = service.predict(smiles)

        return func.HttpResponse(
            body=json.dumps([p.model_dump() for p in prediction]),
            mimetype="application/json",
            status_code=200,
        )
    except Exception as e:
        logging.error(f"Error processing prediction: {e}")
        return _error_response(f"Error processing prediction")


def _error_response(error: str) -> func.HttpResponse:
    return func.HttpResponse(
        body=json.dumps({"error": error}),
        mimetype="application/json",
        status_code=400,
    )
