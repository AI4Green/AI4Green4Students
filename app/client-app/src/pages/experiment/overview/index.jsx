import {
  HStack,
  Heading,
  VStack,
  Text,
  Icon,
  IconButton,
  Avatar,
  useDisclosure,
  LinkBox,
  LinkOverlay,
} from "@chakra-ui/react";
import { FaCheckCircle, FaEdit, FaExchangeAlt } from "react-icons/fa";
import { Link, useNavigate } from "react-router-dom";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { NotificationBadge } from "components/NotificationBadge";
import { Header } from "components/experiment/section/Header";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { STAGES } from "constants/stages";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { useState } from "react";
import { ActionButton } from "components/ActionButton";
import { MoveStageModal } from "components/experiment/modal/MoveStageModal";

const Section = ({ section, path, index }) => {
  const { name, approved, comments } = section;
  const ariaQualifier = comments == 1 ? "is " : "are ";
  const ariaPlural = comments == 1 ? "" : "s";

  return (
    <LinkBox w="full">
      <HStack
        w="full"
        borderBottomWidth={1}
        p={2}
        borderRadius={5}
        gap={2}
        _hover={{
          bg: "gray.50",
        }}
      >
        <Text>{index + 1}</Text>
        <VStack align="start" spacing={0.2}>
          <Heading as="h4" size="md">
            <LinkOverlay
              as={Link}
              to={path}
              aria-label={
                name +
                (approved ? ". Item is approved" : ". Incomplete/Unapproved") +
                (comments >= 1
                  ? ". There " +
                    ariaQualifier +
                    comments +
                    " comment" +
                    ariaPlural +
                    " on this item"
                  : "")
              }
            >
              {name}
            </LinkOverlay>
          </Heading>
        </VStack>

        <HStack justifyContent="flex-end" flex={1}>
          {comments >= 1 && !approved ? (
            <NotificationBadge
              count={comments > 9 ? "9+" : comments}
              as={Text}
              to={path}
            />
          ) : approved ? (
            <IconButton
              as={Text}
              to={path}
              isRound
              variant="ghost"
              aria-label={name + ". approved"}
              size="lg"
              icon={<Icon as={FaCheckCircle} color="green.500" boxSize={5} />}
            />
          ) : (
            <IconButton
              as={Text}
              to={path}
              isRound
              variant="ghost"
              aria-label={name + ". Incomplete/Unapproved"}
              size="lg"
              icon={<Icon as={FaEdit} boxSize={5} color="gray.600" />}
            />
          )}
        </HStack>
      </HStack>
    </LinkBox>
  );
};

export const Overview = ({
  sections,
  headerItems: { header, subHeader, owner, overviewTitle },
  InstructorAction,
}) => {
  const isInstructor = useIsInstructor();
  const ExperimentAuthor = () => (
    <HStack pb={2}>
      <Avatar name={owner} size="sm" />
      <Text fontSize="md" color="gray.600">
        {owner}
      </Text>
    </HStack>
  );

  return (
    <ExperimentLayout>
      <Header
        header={header}
        subHeader={subHeader}
        overviewTitle={overviewTitle}
        actionSection={
          <>
            <ExperimentAuthor />
            {isInstructor && InstructorAction}
          </>
        }
      />
      <VStack w="lg">
        {sections && sections.length >= 1 ? (
          sections
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((section, index) => (
              <Section
                key={section.id}
                section={section}
                path={section.path}
                index={index}
              />
            ))
        ) : (
          <Text fontSize="lg">No sections available</Text>
        )}
      </VStack>
    </ExperimentLayout>
  );
};

export const InstructorAction = ({
  record,
  isEverySectionApproved,
  isPlan,
  isLiteratureReview,
  isReport,
}) => {
  const [modalActionProps, setModalActionProps] = useState({
    modalTitle: "Confirmation",
    fixedNextStage: null,
    successMessage: "Success",
  });

  const {
    isOpen: isOpenAdvanceStage,
    onOpen: onOpenAdvanceStage,
    onClose: onCloseAdvanceStage,
  } = useDisclosure();
  const navigate = useNavigate();

  const actions = createInstructorActions({
    record,
    isEverySectionApproved,
    onOpenAdvanceStage,
    setModalActionProps,
    navigate,
  });

  return (
    <>
      <ActionButton
        actions={actions}
        size="sm"
        variant="outline"
        colorScheme="pink"
      />
      {isOpenAdvanceStage && (
        <MoveStageModal
          isModalOpen={isOpenAdvanceStage}
          onModalClose={onCloseAdvanceStage}
          record={record}
          isPlan={isPlan}
          isLiteratureReview={isLiteratureReview}
          isReport={isReport}
          mutate={record.mutate}
          {...modalActionProps}
        />
      )}
    </>
  );
};

const createInstructorActions = ({
  record,
  isEverySectionApproved,
  onOpenAdvanceStage,
  setModalActionProps,
}) => {
  return {
    requestChanges: {
      isEligible: () =>
        record.permissions.includes(STAGES_PERMISSIONS.InstructorCanComment),
      icon: <FaExchangeAlt />,
      label: "Request Changes",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Request Changes",
          modalMessage:
            "Do you wish to proceed with requesting changes for the following?",
          fixedNextStage: STAGES.AwaitingChanges,
          successMessage: "Request changes succeeded",
          failMessage: "Request changes failed",
        });
        onOpenAdvanceStage();
      },
    },
    cancelRequestChanges: {
      isEligible: () => record.stage === STAGES.AwaitingChanges,
      icon: <FaExchangeAlt />,
      label: "Cancel Request Changes",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Request Changes",
          modalMessage:
            "Do you wish to proceed with cancelling the request changes for the following?",
          fixedNextStage: STAGES.InReview,
          successMessage: "Request changes cancellation succeeded",
          failMessage: "Request changes cancellation failed",
        });
        onOpenAdvanceStage();
      },
    },
    markAsApproved: {
      isEligible: () =>
        record.permissions.includes(STAGES_PERMISSIONS.InstructorCanComment) &&
        isEverySectionApproved,
      icon: <FaCheckCircle />,
      label: "Mark as approved",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Mark as approved",
          modalMessage:
            "Do you wish to proceed with marking the following as approved?",
          fixedNextStage: STAGES.Approved,
          successMessage: "Mark as approved succeeded",
          failMessage: "Mark as approved failed",
        });
        onOpenAdvanceStage();
      },
    },
    cancelApproval: {
      isEligible: () => record.stage === STAGES.Approved,
      icon: <FaCheckCircle />,
      label: "Cancel Approval",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Approval",
          modalMessage:
            "Do you wish to proceed with cancelling the approval for the following?",
          fixedNextStage: STAGES.InReview,
          successMessage: "Approval cancellation succeeded",
          failMessage: "Approval cancellation  failed",
        });
        onOpenAdvanceStage();
      },
    },
  };
};
