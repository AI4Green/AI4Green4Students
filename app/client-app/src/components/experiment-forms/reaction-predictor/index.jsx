import {
  Box,
  Button,
  FormControl,
  useDisclosure,
  VStack,
} from "@chakra-ui/react";
import { DataTable } from "components/core/data-table";
import { FormHelpError, FormikInput } from "components/core/forms";
import { Modal } from "components/core/modal";
import { useBackendApi } from "contexts";
import { Form, Formik, useField } from "formik";
import { ContentPage } from "pages/content";
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
  const { isOpen, onOpen, onClose } = useDisclosure();

  const [field, , helpers] = useField(name);
  const { predictions: action } = useBackendApi();

  const handlePrediction = async ({ smiles }) => {
    setIsLoading(true);
    try {
      const result = await action.predictProducts(smiles);
      helpers.setValue({ smiles, predictions: result });
      feedback && setFeedback(null);
    } catch (e) {
      const error = await e.response?.json();
      setFeedback(error?.message || "Something went wrong.");
    } finally {
      setIsLoading(false);
    }
  };

  const modalBody = <ContentPage contentKey="reactionprediction" />;

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
                <VStack align="flex-start" spacing={8}>
                  <Button
                    size="xs"
                    onClick={onOpen}
                    leftIcon={<LuLoaderPinwheel />}
                    variant="link"
                  >
                    Learn about Reaction Predictions
                  </Button>
                  {isOpen && (
                    <Modal
                      body={modalBody}
                      title="Learn about Reaction Predictions"
                      onAction={onClose}
                      isOpen={isOpen}
                      contentMaxW="80vw"
                      contentMaxH="90vh"
                      bodyMaxH="70vh"
                      bodyOverflowY="auto"
                      cancelBtnEnable={false}
                    />
                  )}
                  <FormikInput
                    name="smiles"
                    label={label}
                    isDisabled={isDisabled}
                    isRequired
                  />
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
                </VStack>
              </Form>

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
