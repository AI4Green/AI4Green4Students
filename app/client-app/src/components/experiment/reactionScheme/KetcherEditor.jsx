import { Button, FormControl, Text, VStack } from "@chakra-ui/react";
import { useField } from "formik";
import { useRef, useState } from "react";
import { reactionSmilesToReactantsAndProductsSmiles } from "helpers/sketcher-utils";
import { replaceSmilesSymbols } from "helpers/sketcher-utils";
import { useBackendApi } from "contexts/BackendApi";
import { FormHelpError } from "components/forms/FormHelpError";
import { GiMaterialsScience } from "react-icons/gi";

const KETCHER_EDITOR_INITALS_VALUES = {
  sketcherSmiles: "", // smiles from the Ketcher
  reactants: [], // reactants extracted using the smiles
  products: [], // products extracted using the smiles
  smiles: "", // smiles from the Ketcher with replaced symbols
  data: null, // data from AI4Green
};

export const KetcherEditor = ({ name, isDisabled }) => {
  const [field, meta, helpers] = useField(name);

  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const ketcherIframe = useRef(null);

  const { ai4Green: action } = useBackendApi();

  const ketcherWindow = ketcherIframe.current?.contentWindow;

  const handleKetcherOnLoad = async () => {
    const { sketcherSmiles } = field.value || KETCHER_EDITOR_INITALS_VALUES;
    if (ketcherWindow && sketcherSmiles) {
      await ketcherWindow.ketcher.setMolecule(sketcherSmiles);
    }
  };

  // Extract smiles from sketcher and use it to get data from AI4Green to populate the table
  const handleDataGenerate = async () => {
    if (!ketcherWindow) return;

    const sketcherSmiles = await ketcherWindow.ketcher.getSmiles();
    const { reactants, products } =
      reactionSmilesToReactantsAndProductsSmiles(sketcherSmiles);

    const smiles = replaceSmilesSymbols(sketcherSmiles);

    if (!reactants || !products || !smiles) {
      setFeedback("Invalid reaction provided.");
      helpers.setValue(KETCHER_EDITOR_INITALS_VALUES);
      return;
    }

    try {
      setIsLoading(true);
      const response = await action.process(reactants, products, smiles);
      if (response.status === 200) {
        const res = await response.json();
        const { data } = res;

        helpers.setValue({ sketcherSmiles, reactants, products, smiles, data });
        feedback && setFeedback(null);
      }
    } catch (error) {
      setFeedback(error?.message ?? "Something went wrong.");
      helpers.setValue(KETCHER_EDITOR_INITALS_VALUES);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <FormControl isRequired id={field.name} isInvalid={Boolean(feedback)}>
      <VStack minW="full" align="flex-start">
        <Text as="b">Reaction Sketcher</Text>
        <iframe
          ref={ketcherIframe}
          src="/js/ketcher/index.html"
          title="Ketcher App"
          width="100%"
          height="500px"
          allowFullScreen
          onLoad={handleKetcherOnLoad}
          style={{ pointerEvents: isLoading || isDisabled ? "none" : "auto" }}
        />
        {!isDisabled && (
          <Button
            leftIcon={<GiMaterialsScience />}
            isLoading={isLoading}
            disabled={isLoading}
            colorScheme="purple"
            size="sm"
            onClick={handleDataGenerate}
          >
            Generate reaction data
          </Button>
        )}
        <FormHelpError
          isInvalid={Boolean(feedback)}
          error={feedback}
          collapseEmpty
          replaceHelpWithError
        />
      </VStack>
    </FormControl>
  );
};
