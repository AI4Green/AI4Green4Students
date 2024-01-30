import json
import azure.functions as func
import logging

# Processess data from the Ketcher or MarvinJS editor and generates data for the reaction table.
def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info("Python HTTP trigger function processed a request.")

    # get the SMILES string of reactants and products from the args and then replace the symbols
    # reactants0 = req.params.get("reactants")
    # products0 = req.params.get("products")
    # if not reactants0 or products0:
    #     return missing_data_response()

    name = req.params.get("name")

    if name:
        return func.HttpResponse(
            f"Hello, {name}. This HTTP triggered function executed successfully.",
            status_code=200,
        )
    else:
        return func.HttpResponse(
            "Pass a name in the query string or in the request body for a personalized response.",
            status_code=400,
        )


def missing_data_response():
    return func.HttpResponse(
        body=json.dumps({"error": "Missing data!"}),
        mimetype="application/json",
        status_code=400,
    )
