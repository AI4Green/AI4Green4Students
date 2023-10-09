import {
  Alert,
  AlertIcon,
  Stack,
  HStack,
  Heading,
  VStack,
  Text,
  Button,
  Icon,
  useToast,
} from "@chakra-ui/react";
import { useState, useRef } from "react";
import { FaPlus, FaFlask } from "react-icons/fa";
import { useExperiment } from "api/experiments";
import { useParams } from "react-router-dom";
import { ExperimentPlan } from "components/experiment/ExperimentPlan";
import { useBackendApi } from "contexts/BackendApi";

const ExperimentLayout = ({ children }) => (
  <Stack align="stretch" w="100%" alignItems="center">
    <VStack
      m={4}
      p={4}
      align="stretch"
      w={{ base: "95%", md: "85%", lg: "75%", xl: "60%" }}
      spacing={4}
    >
      <VStack borderWidth={1} px={5} py={2} borderRadius={7} spacing={4}>
        {children}
      </VStack>
    </VStack>
  </Stack>
);

export const IndividualExperimentPlan = () => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { experimentId } = useParams();
  const { data: experiment, mutate } = useExperiment(experimentId);
  const { experiments: action } = useBackendApi();
  const formRef = useRef();
  const toast = useToast();

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      // only include non-empty values
      const payload = Object.entries(values).reduce((a, [k, v]) => {
        if (v !== "") {
          a[k] = v;
        }
        return a;
      }, {});

      // cleaning and JSON stringifying references
      const referencesJSON = JSON.stringify(
        payload.references?.map((r) => ({
          id: Number.isInteger(r.id) ? r.id : undefined, // no id means new reference
          order: r.order,
          content: r.content,
        }))
      );

      const response = await action.edit({
        ...payload,
        id: experiment.id,
        references: referencesJSON,
      });

      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          position: "top",
          title: "Experiment saved",
          status: "success",
          duration: 1500,
          isClosable: true,
        });
        mutate();
      }
    } catch (e) {
      console.error(e);
      setFeedback("Something went wrong!");
    }
  };

  const Header = () => (
    <HStack w="100%" borderBottomWidth={1}>
      <VStack align="start" my={2}>
        <Heading as="h2" size="md" fontWeight="semibold" color="blue.600">
          <Icon as={FaFlask} /> {experiment.title}
        </Heading>
        <HStack spacing={2}>
          <Heading as="h2" size="xs" fontWeight="semibold">
            {experiment.projectName}
          </Heading>
        </HStack>
      </VStack>

      <HStack flex={1} justifyContent="flex-end">
        <Button
          colorScheme="green"
          leftIcon={<FaPlus />}
          size="sm"
          isLoading={isLoading}
          onClick={() => formRef.current.submitForm()}
        >
          <Text fontSize="sm" fontWeight="semibold">
            Save
          </Text>
        </Button>
      </HStack>
    </HStack>
  );

  return (
    <ExperimentLayout>
      {feedback && (
        <Alert status="error">
          <AlertIcon />
          {feedback}
        </Alert>
      )}
      <Header />
      <ExperimentPlan
        experiment={experiment}
        formRef={formRef}
        onSubmit={handleSubmit}
      />
    </ExperimentLayout>
  );
};
