from PredictProduct.models.dto import PubchemName
import requests
import time
from functools import lru_cache
    
class PubchemAPI:
    def __init__(self, requests_per_second=3):
        self.requests_per_second = requests_per_second
        self.last_request_time = 0
        self.session = requests.Session()
        self.session.headers.update({
            'User-Agent': 'FastAPI-Molecular-Prediction/1.0 (research use)'
        })

    @lru_cache(maxsize=1000)
    def get_name_from_pubchem(self, smiles: str) -> PubchemName:
        self._rate_limit()
        unavailable = PubchemName(iupac_name="Unavailable", synonyms=[])
        
        base_url = "https://pubchem.ncbi.nlm.nih.gov/rest/pug"

        # Step 1: Get the CID (compound ID) from the SMILES
        cid_url = self._urlpathjoin(base_url, "compound", "smiles", smiles, "cids", "JSON")
        cid_response = self.session.get(cid_url)

        if cid_response.status_code != 200:
            return unavailable

        try:
            cid_json = cid_response.json()
            cid = cid_json["IdentifierList"]["CID"][0]
        except (KeyError, IndexError, TypeError):
            return unavailable

        # Check if cid is None or empty
        if not cid:
            return unavailable

        # Step 2: Use CID to get IUPAC name and synonyms
        iupac_url = self._urlpathjoin(base_url, "compound", "cid", str(cid), "property", "IUPACName", "JSON")
        synonyms_url = self._urlpathjoin(base_url, "compound", "cid", str(cid), "synonyms", "JSON")

        iupac_name = "Unavailable"
        synonyms: list[str] = []

        iupac_response = self.session.get(iupac_url)
        if iupac_response.status_code == 200:
            try:
                iupac_name = iupac_response.json()["PropertyTable"]["Properties"][0]["IUPACName"]
            except (KeyError, IndexError):
                pass

        synonyms_response = self.session.get(synonyms_url)
        if synonyms_response.status_code == 200:
            try:
                synonyms = synonyms_response.json()["InformationList"]["Information"][0]["Synonym"]
            except (KeyError, IndexError):
                pass

        return PubchemName(iupac_name=iupac_name, synonyms=synonyms)
    
    def _rate_limit(self):
        """Ensure we don't exceed rate limits"""
        current_time = time.time()
        time_since_last = current_time - self.last_request_time
        min_interval = 1.0 / self.requests_per_second
        
        if time_since_last < min_interval:
            time.sleep(min_interval - time_since_last)
        
        self.last_request_time = time.time()

    def _urlpathjoin(self, *pieces):
        """
        Naive URL (path only) joiner,
        simply to handle uncertainty of
        trailing and leading slashes between segments
        """
        return "/".join(str(s).strip("/") for s in pieces if s is not None)