import { Box, Button, FormControl } from "@chakra-ui/react";
import { DataTable } from "components/core/data-table";
import { FormHelpError, FormikInput } from "components/core/forms";
import { useBackendApi } from "contexts";
import { Form, Formik, useField } from "formik";
import { useState } from "react";
import { LuLoaderPinwheel } from "react-icons/lu";

import { columns } from "./columns";

export const ReactionPredictor = ({
  name,
  label = "Input SMILES",
  isDisabled,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const [field, , helpers] = useField(name);
  const { predictions: action } = useBackendApi();

  const handlePrediction = async ({ smiles }) => {
    setIsLoading(true);
    try {
      const result = await action.predictProducts(smiles);
      helpers.setValue({ smiles, predictions: result });
      feedback && setFeedback(null);
    } catch {
      setFeedback("Something went wrong.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Box w="full" align="flex-start">
      <FormControl id={field.name} isInvalid={Boolean(feedback)}>
        <Formik
          enableReinitialize
          initialValues={{
            ...(field.value || { smiles: "", predictions: [] }),
          }}
          onSubmit={handlePrediction}
        >
          {({ handleSubmit, values }) => (
            <Box w="full" borderRadius={7} borderWidth={1} p={4}>
              <Form>
                <FormikInput
                  name="smiles"
                  label={label}
                  isDisabled={isDisabled}
                  isRequired
                />
              </Form>
              <Button
                isLoading={isLoading}
                loadingText="Predicting"
                onClick={handleSubmit}
                colorScheme="green"
                size="sm"
                px={4}
                leftIcon={<LuLoaderPinwheel />}
                isDisabled={values.smiles === "" || isLoading || isDisabled}
                hidden={isDisabled}
              >
                Predict
              </Button>
              <FormHelpError
                isInvalid={Boolean(feedback)}
                error={feedback}
                collapseEmpty
                replaceHelpWithError
              />

              {field.value.predictions && (
                <DataTable columns={columns} data={field.value.predictions} />
              )}
            </Box>
          )}
        </Formik>
      </FormControl>
    </Box>
  );
};
