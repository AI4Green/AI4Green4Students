import { Box, Button, VStack, Heading } from "@chakra-ui/react";
import { TextField } from "components/forms/TextField";
import { Formik, Form } from "formik";
import { number, object } from "yup";

export const WasteIntensity = () => {
  return (
    <Box p={4}>
      <VStack spacing={4} align="stretch">
        <Heading>Waste Intensity Calculator</Heading>
        <Formik
          initialValues={{ waste: "", output: "" }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema}
        >
          <Form>
            <TextField
              name="waste"
              label="Waste Produced"
              type="number"
              placeholder="Enter waste produced"
            />

            <TextField
              name="output"
              label="Productivity Output"
              type="number"
              placeholder="Enter productivity output"
            />

            <Button type="submit" mt={4} colorScheme="teal">
              Calculate Waste Intensity
            </Button>
          </Form>
        </Formik>
      </VStack>
    </Box>
  );
};

const validationSchema = object().shape({
  waste: number()
    .required("Waste Produced is required")
    .typeError("Waste Produced must be a numeric value"),
  output: number()
    .required("Productivity Output is required")
    .typeError("Productivity Output must be a numeric value")
    .moreThan(0, "Productivity Output must be greater than 0"),
});

const handleSubmit = (values, { setSubmitting }) => {
  const wasteValue = parseFloat(values.waste);
  const outputValue = parseFloat(values.output);

  const intensity = wasteValue / outputValue;
  alert(`Waste Intensity: ${intensity}`);

  // Reset submitting state after processing
  setSubmitting(false);
};
