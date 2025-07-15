import { VStack, Text, HStack, Icon, Box } from "@chakra-ui/react";
import { Modal } from "components/core/Modal";
import { TITLE_ICON_COMPONENTS } from "constants";
import { Formik, Form } from "formik";
import { FormattedTextInput } from "components/core/forms";
import { useNoteFeedback } from "api/note";

export const ViewModal = ({ isModalOpen, onModalClose, note }) => {
  const { data } = useNoteFeedback(note.id);

  const modalBody = (
    <VStack>
      <HStack spacing={5} w="full">
        <Icon
          as={TITLE_ICON_COMPONENTS.Note}
          color={"green.500"}
          fontSize="5xl"
        />
        <VStack align="flex-start" flex={1}>
          <Text fontWeight="medium">{note.reactionName}</Text>
        </VStack>
      </HStack>
      <Box w="full">
        <Formik initialValues={{ feedback: data?.feedback || "" }}>
          <Form>
            <FormattedTextInput name="feedback" label="" isDisabled />
          </Form>
        </Formik>
      </Box>
    </VStack>
  );

  return (
    <Modal
      body={modalBody}
      title="Feedback"
      actionBtnColorScheme="green"
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
