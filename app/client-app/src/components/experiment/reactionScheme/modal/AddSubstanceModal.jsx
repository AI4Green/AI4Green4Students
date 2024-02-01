import { useRef } from "react";
import { ErrorMessage, Form, Formik } from "formik";
import { BasicModal } from "components/BasicModal";
import { object, string } from "yup";
import { Box, FormLabel, Select, Text, VStack } from "@chakra-ui/react";
import AsyncSelect from "react-select/async";
import { useBackendApi } from "contexts/BackendApi";

export const AddSubstanceModal = ({
  isModalOpen,
  onModalClose,
  setTableData,
  isAddingSolvent,
}) => {
  const formRef = useRef();

  const { ai4Green: action } = useBackendApi();

  const handleAddSubstance = async (values) => {
    let data = {};
    try {
      const response = await action.getReagent(values.substance);
      if (response.status === 200) {
        const res = await response.json();
        data = {
          ...res,
          ...values,
        };
      }
    } catch (error) {
      console.error(error);
    }

    const accessorKeyValues = {
      substanceType: data?.substanceType,
      substancesUsed: data?.substance,
      molWeight: data?.molecularWeight,
      density: data?.density,
      hazards: data?.hazards,
      limiting: false,
    };

    setTableData((old) => [...old, { ...accessorKeyValues }]);
    onModalClose();
  };

  const loadReagents = async (inputValue, callback) => {
    try {
      const response = await action.getPartialReagents(inputValue);
      let data = [];
      if (response.status === 200) {
        const responseData = await response.json();
        data = responseData?.map((item) => ({
          value: item.name,
          label: item.name,
        }));
      }
      callback(data);
    } catch (error) {
      console.error(error);
      callback([]);
    }
  };

  const Modal = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{
        substance: "",
        substanceType: "",
      }}
      onSubmit={handleAddSubstance}
      validationSchema={validationSchema()}
    >
      {({ values, setFieldValue }) => (
        <Form noValidate>
          <VStack align="stretch" spacing={4}>
            <Box>
              <FormLabel>
                <Text as="b">Substance type</Text>
              </FormLabel>
              <Select
                size="md"
                value={values.substanceType}
                onChange={(e) => setFieldValue("substanceType", e.target.value)}
                placeholder="Select substance type"
              >
                <option value="Reagent">Reagent</option>
                <option value="Solvent">Solvent</option>
              </Select>
              <ErrorMessage name="substanceType">
                {(msg) => (
                  <Text fontSize="xs" color="red.500">
                    {msg}
                  </Text>
                )}
              </ErrorMessage>
            </Box>
            <Box>
              <FormLabel>
                <Text as="b">Substance</Text>
              </FormLabel>
              <AsyncSelect
                cacheOptions
                loadOptions={
                  values.substanceType.toLowerCase() === "reagent"
                    ? loadReagents
                    : () => []
                }
                defaultOptions
                placeholder="Start typing to search for a substance"
                onChange={(option) => {
                  setFieldValue("substance", option?.value || "");
                }}
              />
            </Box>
          </VStack>
        </Form>
      )}
    </Formik>
  );
  return (
    <BasicModal
      body={Modal}
      title="Add new substance"
      actionBtnCaption="Add"
      onAction={() => formRef.current.handleSubmit()}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};

const validationSchema = () =>
  object().shape({
    substance: string().required("Title is required"),
    substanceType: string().required("Substance type is required"),
  });
