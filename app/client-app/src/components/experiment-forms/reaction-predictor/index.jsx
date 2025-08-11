import { Formik, Form, useField } from "formik";
import { Box, Button } from "@chakra-ui/react";
import { FormikInput } from "components/core/forms";
import { LuLoaderPinwheel } from "react-icons/lu";
import { useBackendApi } from "contexts";
import { useState } from "react";
import { DataTable } from "components/core/data-table";
import { columns } from "./columns";

export const ReactionPredictor = ({
  name,
  label = "Input SMILES",
  isDisabled,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const [field, meta, helpers] = useField(name);
  const { predictions: action } = useBackendApi();

  const handlePrediction = async ({ smiles }) => {
    setIsLoading(true);
    try {
      const result = await action.predictProducts(smiles);
      helpers.setValue({ smiles, predictions: result });
      feedback && setFeedback(null);
    } catch (error) {
      setFeedback(error?.message ?? "Something went wroing.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Box w="full" align="flex-start">
      <Formik
        enableReinitialize
        initialValues={{ ...(field.value || { smiles: "", predictions: [] }) }}
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

            {field.value.predictions && (
              <DataTable columns={columns} data={field.value.predictions} />
            )}
          </Box>
        )}
      </Formik>
    </Box>
  );
};
