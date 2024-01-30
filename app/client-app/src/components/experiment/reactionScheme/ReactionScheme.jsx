import { Button, VStack } from "@chakra-ui/react";
import { Formik, useField } from "formik";
import { KetcherEditor } from "./KetcherEditor";
import { ReactionTable } from "./ReactionTable";
import { useEffect, useRef } from "react";
import { TextAreaField } from "components/forms/TextAreaField";
import { reactionSmilesToReactantsAndProductsSmiles } from "helpers/sketcher-utils";
import { replaceSmilesSymbols } from "helpers/sketcher-utils";
import { useBackendApi } from "contexts/BackendApi";

export const ReactionScheme = ({ name }) => {
  const [field, meta, helpers] = useField(name);

  const { ai4Green: action } = useBackendApi();

  const ketcherIframeRef = useRef(null);

  const ketcherWindow = ketcherIframeRef.current?.contentWindow;

  const handleKetcherOnLoad = async () => {
    const { sketcherSmiles } = field.value.reactionSchemeSketch;
    if (ketcherWindow && sketcherSmiles) {
      await ketcherWindow.ketcher.setMolecule(sketcherSmiles);
    }
  };

  const handleKetcherUpdate = async () => {
    if (!ketcherWindow) return;

    const sketcherSmiles = await ketcherWindow.ketcher.getSmiles();
    let reactionSchemeSketch = {};

    if (sketcherSmiles) {
      const { reactants, products } =
        reactionSmilesToReactantsAndProductsSmiles(sketcherSmiles);
      const smiles = replaceSmilesSymbols(sketcherSmiles);
      const response = await action.process(reactants, products, smiles);
      console.log(response);

      reactionSchemeSketch = {
        sketcherSmiles,
        reactants,
        products,
        smiles,
      };
    }

    helpers.setValue({
      ...field.value,
      reactionSchemeSketch: {
        sketcherSmiles: "",
        reactants: [],
        products: [],
        smiles: "",
        ...reactionSchemeSketch,
      },
    });
  };

  return (
    <Formik
      enableReinitialize
      initialValues={{
        reactionSchemeSketch: field.value?.reactionSchemeSketch || {}, // sketcher data
        reactionDescription: field.value?.reactionDescription || "", // reaction description
        reactionTable: field.value?.reactionTable || [], // table data based on reaction drawing
      }}
    >
      {({ values }) => {
        useEffect(() => {
          // update field value when values changes. reset description and table data if sketcher is empty
          helpers.setValue({
            reactionSchemeSketch: values.reactionSchemeSketch,
            reactionDescription: values.reactionSchemeSketch?.sketcherSmiles
              ? values.reactionDescription
              : "",
            reactionTable: values.reactionSchemeSketch?.sketcherSmiles
              ? values.reactionTable
              : [],
          });
        }, [values]);

        return (
          <VStack minW="full" spacing={5}>
            <KetcherEditor
              ref={ketcherIframeRef}
              width="100%"
              height="500px"
              allowFullScreen
              onLoad={handleKetcherOnLoad}
            />
            <Button onClick={handleKetcherUpdate}>Generate</Button>

            {/* {values.reactionSchemeSketch?.sketcherSmiles && (
              <VStack minW="full" spacing={5} borderWidth={1} p={2}>
                <TextAreaField
                  name="reactionDescription"
                  title="Reaction Description"
                  placeholder="Enter reaction description"
                  isRequired
                />
                <ReactionTable
                  name="reactionTable"
                  reactionSchemeSketchData={values.reactionSchemeSketch}
                />
              </VStack>
            )} */}
          </VStack>
        );
      }}
    </Formik>
  );
};
