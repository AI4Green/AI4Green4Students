import { Box, Button, VStack, Heading } from "@chakra-ui/react";
import { TextField } from "components/forms/TextField";
import { Formik, Form } from "formik";
import { number, object } from "yup";

export const ReactionMassEfficiency = () => {
  return (
    <Box p={4}>
      <VStack spacing={4} align="stretch">
        <Heading>Reaction Mass Efficiency Calculator</Heading>
        <Formik
          initialValues={{ productMass: "", reactantMass: "" }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema}
        >
          <Form>
            <TextField
              name="productMass"
              label="Mass of Product"
              type="number"
              placeholder="Enter mass of product"
            />

            <TextField
              name="reactantMass"
              label="Total Mass of Reactants used"
              type="number"
              placeholder="Enter total mass of reactants"
            />
            <Button type="submit" mt={4} colorScheme="teal">
              Calculate Reaction Mass Effeciency
            </Button>
          </Form>
        </Formik>
      </VStack>
    </Box>
  );
};

const validationSchema = object().shape({
  productMass: number()
    .required("Mass of Product is required")
    .typeError("Mass of Product must be a numeric value"),
  reactantMass: number()
    .required("Total Mass of Reactants used is required")
    .typeError("Total Mass of Reactants used must be a numeric value")
    .moreThan(0, "Total Mass of Reactants used must be greater than 0"),
});

const handleSubmit = (values, { setSubmitting }) => {
  const productMass = parseFloat(values.productMass);
  const reactantMass = parseFloat(values.reactantMass);

  const rme = (productMass / reactantMass) * 100;
  alert(`Reaction Mass Efficiency: ${rme}`);

  // Reset submitting state after processing
  setSubmitting(false);
};
