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

export const EFactor = () => {
  return (
    <Box p={4}>
      <VStack spacing={4} align="stretch">
        <Heading>E-Factor Calculator</Heading>
        <Formik
          initialValues={{ wasteMass: "", productMass: "" }}
          onSubmit={handleSubmit}
        >
          <Form>
            <Field name="wasteMass">
              {({ field }) => (
                <FormControl>
                  <FormLabel htmlFor="wasteMass">
                    Total Mass of Waste Generated:
                  </FormLabel>
                  <Input
                    {...field}
                    type="number"
                    id="wasteMass"
                    placeholder="Enter total waste mass"
                  />
                </FormControl>
              )}
            </Field>

            <Field name="productMass">
              {({ field }) => (
                <FormControl>
                  <FormLabel htmlFor="productMass">
                    Mass of Product Obtained:
                  </FormLabel>
                  <Input
                    {...field}
                    type="number"
                    id="productMass"
                    placeholder="Enter product mass"
                  />
                </FormControl>
              )}
            </Field>

            <Button type="submit" mt={4} colorScheme="teal">
              Calculate E-Factor
            </Button>
          </Form>
        </Formik>
      </VStack>
    </Box>
  );
};
const handleSubmit = (values, { setSubmitting }) => {
  const wasteMass = parseFloat(values.wasteMass);
  const productMass = parseFloat(values.productMass);

  if (!isNaN(wasteMass) && !isNaN(productMass) && productMass > 0) {
    const eFactor = wasteMass / productMass;
    alert(`E-factor: ${eFactor}`);
  } else {
    alert(
      "Invalid input. Please provide numeric values for waste mass and product mass, and make sure product mass is greater than 0."
    );
  }

  // Reset submitting state after processing
  setSubmitting(false);
};
