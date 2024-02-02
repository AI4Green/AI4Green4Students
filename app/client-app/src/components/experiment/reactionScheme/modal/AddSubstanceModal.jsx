import { useRef } from "react";
import { ErrorMessage, Form, Formik } from "formik";
import { BasicModal } from "components/BasicModal";
import { object, string } from "yup";
import {
  Box,
  FormLabel,
  HStack,
  Icon,
  Text,
  VStack,
  useToast,
} from "@chakra-ui/react";
import AsyncSelect from "react-select/async";
import { useBackendApi } from "contexts/BackendApi";
import { useSolventsList } from "api/ai4green";
import { FaFlask, FaVial } from "react-icons/fa";

export const AddSubstanceModal = ({
  isModalOpen,
  onModalClose,
  setTableData,
  isAddingSolvent,
}) => {
  const formRef = useRef();
  const toast = useToast();

  const { ai4Green: action } = useBackendApi();
  const { data: solvents } = useSolventsList();

  const solventsOptions = solvents?.map((item) => ({
    value: item.name,
    label: item.name,
  }));

  const handleAddSubstance = async (values) => {
    try {
      const data = isAddingSolvent
        ? await action.getSolvent(values.substance)
        : await action.getReagent(values.substance);

      setTableData((old) => [...old, createRowData(data, values)]);
      onModalClose();
    } catch (error) {
      toast({
        title: "An error occurred.",
        description: error.message,
        status: "error",
        duration: 9000,
        isClosable: true,
        direction: "top",
      });
    }
  };

  const loadCompounds = async (inputValue) => {
    try {
      const response = await action.getCompounds(inputValue);
      return response?.map((item) => ({
        value: item.name,
        label: item.name,
      }));
    } catch (error) {
      console.error(error);
      return [];
    }
  };

  const Modal = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{
        substanceType: !isAddingSolvent ? "Reagent" : "Solvent",
        substance: "",
      }}
      onSubmit={handleAddSubstance}
      validationSchema={validationSchema}
    >
      {({ setFieldValue }) => (
        <Form noValidate>
          <VStack align="stretch" spacing={4}>
            <HStack spacing={5}>
              <Icon
                flex={1}
                w="full"
                as={isAddingSolvent ? FaVial : FaFlask}
                color={isAddingSolvent ? "teal" : "pink.600"}
                fontSize="5xl"
              />
              <Box>
                <FormLabel>
                  <Text as="b">Substance</Text>
                </FormLabel>
                <AsyncSelect
                  cacheOptions
                  loadOptions={loadCompounds}
                  defaultOptions={isAddingSolvent ? solventsOptions : []}
                  placeholder="Start typing to search for a substance"
                  onChange={(option) => {
                    setFieldValue("substance", option?.value || "");
                  }}
                />
                <ErrorMessage name="substance">
                  {(msg) => (
                    <Text fontSize="sm" color="red.500">
                      {msg}
                    </Text>
                  )}
                </ErrorMessage>
              </Box>
            </HStack>
          </VStack>
        </Form>
      )}
    </Formik>
  );
  return (
    <BasicModal
      body={Modal}
      title={`Add ${isAddingSolvent ? "Solvent" : "Reagent"}`}
      actionBtnCaption="Add"
      onAction={() => formRef.current.handleSubmit()}
      isOpen={isModalOpen}
      onClose={onModalClose}
      actionBtnColorScheme={isAddingSolvent ? "teal" : "pink"}
    />
  );
};

const createRowData = (data, values) => ({
  manualEntry: true,
  substanceType: data?.substanceType,
  substancesUsed: data?.substance,
  molWeight: data?.molecularWeight,
  density: data?.density,
  hazards: data?.hazards,
  limiting: false,
  ...values,
});

const validationSchema = object().shape({
  substance: string().required("Please select a substance"),
  substanceType: string().required("Substance type is required"),
});
