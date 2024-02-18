import { Box, Button, VStack, Heading } from "@chakra-ui/react";
import { TextField } from "components/forms/TextField";
import { Formik, Form } from "formik";
import { number, object } from "yup";

export const EFactor = () => {
  return (
    <Box p={4}>
      <VStack spacing={4} align="stretch">
        <Heading>E-Factor Calculator</Heading>
        <Formik
          initialValues={{ wasteMass: "", productMass: "" }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema}
        >
          <Form>
            <TextField
              name="wasteMass"
              label="Total Mass of Waste Generated:"
              type="number"
              placeholder="Enter total waste mass"
            />

            <TextField
              name="productMass"
              label="Mass of Product Obtained"
              type="number"
              placeholder="Enter product mass"
            />

            <Button type="submit" mt={4} colorScheme="teal">
              Calculate E-Factor
            </Button>
          </Form>
        </Formik>
      </VStack>
    </Box>
  );
};
const validationSchema = object().shape({
  wasteMass: number()
    .required("Waste Mass is required")
    .typeError("Waste Mass must be a numeric value"),
  productMass: number()
    .required("Product Mass is required")
    .typeError("Product Mass must be a numeric value")
    .moreThan(0, "Product Mass must be greater than 0"),
});

const handleSubmit = (values, { setSubmitting }) => {
  const wasteMass = parseFloat(values.wasteMass);
  const productMass = parseFloat(values.productMass);

  const eFactor = wasteMass / productMass;
  alert(`E-factor: ${eFactor}`);

  // Reset submitting state after processing
  setSubmitting(false);
};
