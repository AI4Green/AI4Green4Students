import logging
import os
from .services.predict import PredictionService
from .helpers.model_runner import ModelRunner
import azure.functions as func

import json

def main(req: func.HttpRequest) -> func.HttpResponse:
    smiles = req.params.get("smiles")
    if not smiles:
        logging.error("Missing required parameter")
        return _error_response("Missing required parameter")
    
    # Load model if not already loaded
    model_dir = os.environ.get("MODEL_DIR", "/mnt/model")
    
    model_runner = ModelRunner(model_dir=model_dir, n_best=30, beam_size=5, use_cuda=False)
    model_runner.load()
    if model_runner is None:
        logging.error("Model is not loaded")
        return _error_response("Model is unavailable")
    
    service = PredictionService(model=model_runner)
    try:
        prediction = service.predict([smiles])    
        
        return func.HttpResponse(
            body=json.dumps(prediction),
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