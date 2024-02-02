import { useRef } from "react";
import { Form, Formik } from "formik";
import { BasicModal } from "components/BasicModal";
import { object, string } from "yup";
import { Box, FormLabel, Text, VStack } from "@chakra-ui/react";
import AsyncSelect from "react-select/async";
import { useBackendApi } from "contexts/BackendApi";
import { useSolventsList } from "api/ai4green";

export const AddSubstanceModal = ({
  isModalOpen,
  onModalClose,
  setTableData,
  isAddingSolvent,
}) => {
  const formRef = useRef();

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

      const rowData = {
        ...data,
        ...values,
      };

      const accessorKeyValues = {
        substanceType: rowData?.substanceType,
        substancesUsed: rowData?.substance,
        molWeight: rowData?.molecularWeight,
        density: rowData?.density,
        hazards: rowData?.hazards,
        limiting: false,
      };

      setTableData((old) => [...old, { ...accessorKeyValues }]);
      onModalClose();
    } catch (error) {
      console.error(error);
    }
  };

  const loadCompounds = async (inputValue, callback) => {
    try {
      const response = await action.getCompounds(inputValue);
      const compounds = response?.map((item) => ({
        value: item.name,
        label: item.name,
      }));

      callback(compounds);
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
        substanceType: !isAddingSolvent ? "Reagent" : "Solvent",
        substance: "",
      }}
      onSubmit={handleAddSubstance}
      validationSchema={validationSchema()}
    >
      {({ setFieldValue }) => (
        <Form noValidate>
          <VStack align="stretch" spacing={4}>
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
