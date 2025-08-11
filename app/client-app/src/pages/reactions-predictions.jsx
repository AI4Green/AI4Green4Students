import { Heading, Stack, VStack } from "@chakra-ui/react";
import { ReactionPredictor } from "components/experiment-forms";
import { Formik } from "formik";

export default function ReactionPredictions() {
  return (
    <Stack w="full" alignItems="center">
      <VStack
        m={4}
        p={4}
        align="stretch"
        minW={{ base: "85%", md: "70%", lg: "60%", xl: "50%" }}
        spacing={4}
      >
        <Heading fontSize="xl">Reaction Predictions</Heading>
        <Formik enableReinitialize initialValues={{ reactionPrediction: "" }}>
          <ReactionPredictor
            name="reactionPrediction"
            label="Input Reactant SMILES"
          />
        </Formik>
      </VStack>
    </Stack>
  );
}
