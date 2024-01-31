import {
  Box,
  FormControl,
  FormLabel,
  Input,
  Button,
  Text,
  VStack,
  Heading,
} from "@chakra-ui/react";
import { Formik, Form, Field } from "formik";
export const WasteIntensity = () => {
  return (
    <Box p={4}>
      <VStack spacing={4} align="stretch">
        <Heading>Waste Intensity Calculator</Heading>
        <Formik
          initialValues={{ waste: "", output: "" }}
          onSubmit={handleSubmit}
        >
          <Form>
            <Field name="waste">
              {({ field }) => (
                <FormControl>
                  <FormLabel htmlFor="waste">Waste Produced:</FormLabel>
                  <Input
                    {...field}
                    type="number"
                    id="waste"
                    placeholder="Enter waste produced"
                  />
                </FormControl>
              )}
            </Field>

            <Field name="output">
              {({ field }) => (
                <FormControl>
                  <FormLabel htmlFor="output">Productivity Output:</FormLabel>
                  <Input
                    {...field}
                    type="number"
                    id="output"
                    placeholder="Enter productivity output"
                  />
                </FormControl>
              )}
            </Field>

            <Button type="submit" mt={4} colorScheme="teal">
              Calculate Waste Intensity
            </Button>
          </Form>
        </Formik>
      </VStack>
    </Box>
  );
};

const handleSubmit = (values, { setSubmitting }) => {
  const wasteValue = parseFloat(values.waste);
  const outputValue = parseFloat(values.output);

  if (!isNaN(wasteValue) && !isNaN(outputValue) && outputValue > 0) {
    const intensity = wasteValue / outputValue;
    alert(`Waste Intensity: ${intensity}`);
  } else {
    alert(
      "Invalid input. Please provide numeric values for waste and output, and make sure output is greater than 0."
    );
  }

  // Reset submitting state after processing
  setSubmitting(false);
};
