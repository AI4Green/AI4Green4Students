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
            <TextField
              name="wasteMass"
              label="Total Mass of Waste Generated:"
              type="number"
              placeholder="Enter total waste mass"
            />

            <TextField name="productMass">
              label= "Mass of Product Obtained" type number placeholder="Enter
              product mass"
            </TextField>

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
