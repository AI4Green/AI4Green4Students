import { useDisclosure } from "@chakra-ui/react";
import { ActionButton } from "components/core/ActionButton";
import { FaEye } from "react-icons/fa";
import { OverviewModal } from "./modal";

export const StudentAction = ({ sections, record, sectionType }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();

  return (
    <>
      <ActionButton
        actions={{
          viewSummary: {
            isEligible: () => true,
            icon: <FaEye />,
            label: "View Summary",
            onClick: onOpen,
          },
        }}
        size="sm"
        variant="outline"
        colorScheme="gray"
      />
      {isOpen && (
        <OverviewModal
          isOpen={isOpen}
          onClose={onClose}
          sections={sections}
          record={record}
          sectionType={sectionType}
        />
      )}
    </>
  );
};
