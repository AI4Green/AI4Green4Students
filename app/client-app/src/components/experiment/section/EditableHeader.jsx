import {
  HStack,
  Heading,
  VStack,
  Icon,
  IconButton,
  useToast,
} from "@chakra-ui/react";
import {
  FaFlask,
  FaEdit,
  FaRegCheckSquare,
  FaRegWindowClose,
} from "react-icons/fa";
import { useState, useRef } from "react";
import { TextField } from "components/forms/TextField";
import { useBackendApi } from "contexts/BackendApi";
import { Formik, Form } from "formik";
import { useTranslation } from "react-i18next";

export const EditableHeader = ({
  experiment,
  header,
  mutate,
  actionSection,
}) => {
  const { experiments: action } = useBackendApi();
  const [isEditing, setIsEditing] = useState(false);
  const formRef = useRef();
  const toast = useToast();
  const { t } = useTranslation();

  const handleSubmit = async (values) => {
    try {
      const response = await action.edit({
        values,
        id: experiment.id,
      });

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          position: "top",
          title: "Experiment title updated",
          status: "success",
          duration: 1500,
          isClosable: true,
        });
        mutate();
      }
    } catch (e) {
      toast({
        position: "top",
        title: t("feedback.error_title"),
        status: "error",
        duration: 1500,
        isClosable: true,
      });
    }
    setIsEditing(false);
  };
  return (
    <HStack w="full" borderBottomWidth={1}>
      <VStack align="start" my={2}>
        <Heading as="h2" size="md" fontWeight="semibold" color="green.600">
          {isEditing ? (
            <Formik
              innerRef={formRef}
              initialValues={{ title: experiment.title }}
              enableReinitialize
              onSubmit={handleSubmit}
            >
              <Form noValidate>
                <HStack>
                  <Icon as={FaFlask} />
                  <TextField name="title" isRequired />
                  <IconButton
                    colorScheme="green"
                    variant="ghost"
                    icon={<FaRegCheckSquare />}
                    onClick={() => formRef.current.handleSubmit()}
                  />
                  <IconButton
                    colorScheme="red"
                    variant="ghost"
                    icon={<FaRegWindowClose />}
                    onClick={() => setIsEditing(false)}
                  />
                </HStack>
              </Form>
            </Formik>
          ) : (
            <>
              <Icon as={FaFlask} />
              {experiment.title}
              <IconButton
                variant="ghost"
                aria-label="Edit experiment title"
                size="sm"
                icon={<Icon as={FaEdit} boxSize={4} color="gray.600" />}
                onClick={() => setIsEditing(true)}
              />
            </>
          )}
        </Heading>
        <HStack spacing={2}>
          <Heading as="h2" size="xs" fontWeight="semibold">
            {experiment.projectName}
          </Heading>
        </HStack>
      </VStack>
      <VStack align="end" my={2} flex={1}>
        <Heading as="h2" size="lg" fontWeight="semibold" color="blue.600">
          {header}
        </Heading>

        {actionSection}
      </VStack>
    </HStack>
  );
};
