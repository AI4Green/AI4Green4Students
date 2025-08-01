import os
import glob
from onmt.translate.translation_server import ServerModel as ONMTServerModel

    
class ModelRunner:
    def __init__(self, model_dir, n_best=1, beam_size=1, use_cuda=False):
        self.model_dir = model_dir
        self.n_best = n_best
        self.beam_size = beam_size
        self.use_cuda = use_cuda
        self.model = None

    def load(self):
        """
        Load the OpenNMT model from checkpoint (.pt file)
        """
        checkpoint_files = glob.glob(os.path.join(self.model_dir, "*.pt"))
        if not checkpoint_files:
            raise FileNotFoundError(f"No .pt checkpoint found in {self.model_dir}")

        checkpoint_file = checkpoint_files[0]

        onmt_config = {
            "models": checkpoint_file,
            "n_best": self.n_best,
            "beam_size": self.beam_size
        }

        self.model = ONMTServerModel(opt=onmt_config, model_id=0, load=True)

    def run(self, data):
        if self.model is None:
            raise RuntimeError("Model has not been loaded.")
        return self.model.run(data)