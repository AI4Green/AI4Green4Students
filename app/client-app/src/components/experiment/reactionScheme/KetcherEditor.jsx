import { Button, Text, VStack } from "@chakra-ui/react";
import { useField } from "formik";
import { useRef } from "react";
import { reactionSmilesToReactantsAndProductsSmiles } from "helpers/sketcher-utils";

export const KetcherEditor = ({ name, isRequired }) => {
  const [field, meta, helpers] = useField(name);

  const ketcherIframe = useRef(null);

  const getKetcherInstance = () =>
    ketcherIframe.current?.contentWindow?.ketcher;

  const getSketcherSmiles = async () => await getKetcherInstance().getSmiles();

  const setInitialSmiles = async () => {
    const ketcher = getKetcherInstance();
    const { sketcherSmiles } = field.value;
    if (ketcher && sketcherSmiles) {
      await ketcher.setMolecule(sketcherSmiles);
    }
  };

  const updateSmilesValue = async () => {
    const sketcherSmiles = await getSketcherSmiles();

    if (!sketcherSmiles) {
      // reset if no sketcherSmiles
      helpers.setValue({
        sketcherSmiles: "",
        smiles: "",
        reactants: [],
        products: [],
      });
      return;
    }

    const { reactants, products, smiles } =
      reactionSmilesToReactantsAndProductsSmiles(sketcherSmiles);

    helpers.setValue({ sketcherSmiles, smiles, reactants, products });
  };

  return (
    <VStack minW="full" align="flex-start">
      <Text as="b">Reaction Sketcher</Text>
      <iframe
        ref={ketcherIframe}
        src="/js/ketcher/index.html"
        title="Ketcher App"
        width="100%"
        height="500px"
        allowFullScreen
        onLoad={setInitialSmiles}
      />
      <Button onClick={updateSmilesValue}>Save</Button>
    </VStack>
  );
};
