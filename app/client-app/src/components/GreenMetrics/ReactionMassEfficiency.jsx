import React from "react";
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

export const ReactionMassEfficiency = () => {
  return (
    <Box p={4}>
      <VStack spacing={4} align="stretch">
        <Heading>Reaction Mass Efficiency Calculator</Heading>
        <Formik
          initialValues={{ productMass: "", reactantMass: "" }}
          onSubmit={handleSubmit}
        >
          <Form>
            <Field name="productMass">
              {({ field }) => (
                <FormControl>
                  <FormLabel htmlFor="productMass">Mass of Product:</FormLabel>
                  <Input
                    {...field}
                    type="number"
                    id="wasteMass"
                    placeholder="Enter mass of product"
                  />
                </FormControl>
              )}
            </Field>

            <Field name="reactantMass">
              {({ field }) => (
                <FormControl>
                  <FormLabel htmlFor="reactantMass">
                    Total Mass of Reactants used:
                  </FormLabel>
                  <Input
                    {...field}
                    type="number"
                    id="reactantMass"
                    placeholder="Enter total mass of reactants"
                  />
                </FormControl>
              )}
            </Field>

            <Button type="submit" mt={4} colorScheme="teal">
              Calculate Reaction Mass Effeciency
            </Button>
          </Form>
        </Formik>
      </VStack>
    </Box>
  );
};

const handleSubmit = (values, { setSubmitting }) => {
  const productMass = parseFloat(values.productMass);
  const reactantMass = parseFloat(values.reactantMass);

  if (!isNaN(productMass) && !isNaN(reactantMass) && reactantMass > 0) {
    const rme = (productMass / reactantMass) * 100;
    alert(`Reaction Mass Efficiency: ${rme}`);
  } else {
    alert(
      "Invalid input. Please provide numeric values for product mass and reactant mass, and make sure product mass is greater than 0."
    );
  }

  // Reset submitting state after processing
  setSubmitting(false);
};
