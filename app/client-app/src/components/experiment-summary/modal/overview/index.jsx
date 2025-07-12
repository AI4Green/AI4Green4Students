import {
  Avatar,
  Box,
  Divider,
  HStack,
  Icon,
  Text,
  VStack,
} from "@chakra-ui/react";
import { useNoteSection, usePlanSection, useReportSection } from "api";
import { Modal } from "components/core/Modal";
import { initialValues } from "components/section-form";
import { SECTION_TYPES } from "constants";
import { Fragment, useRef } from "react";
import { MdDownload } from "react-icons/md";
import { FieldResponse, TriggerFieldResponse } from "./field-response";
import html2pdf from "html2pdf.js";

export const OverviewModal = ({
  isOpen,
  onClose,
  sectionType,
  sections,
  record,
}) => {
  // Here we Generate PDF before Download
  const modalRef = useRef();

  const handleDownloadPdf = () => {
    const element = modalRef.current;
    const ownerName = record.ownerName.replace(/\s+/g, "_");

    html2pdf()
      .from(element)
      .set({
        margin: 1,
        filename: `${sectionType}_Overview_${ownerName}.pdf`,
        html2canvas: { scale: 2 },
        jsPDF: { orientation: "portrait" },
      })
      .save();
  };

  const modalBody = (
    <VStack align="start" spacing={6} ref={modalRef}>
      <Details
        title={record.title}
        project={record.projectName}
        owner={record.ownerName}
      />

      {/* Sections Overview Section */}
      <Box p={4} w="full">
        <Text fontWeight="semibold" fontSize="lg" mb={2}>
          Sections Overview
        </Text>
        <Divider mb={4} />
        <VStack align="start" spacing={4}>
          {sections && sections.length > 0 ? (
            sections.map((section, index) => (
              <Fragment key={section.id}>
                <Box w="full">
                  <Text fontWeight="medium">
                    {index + 1}. {section.name}
                  </Text>
                  <SectionFieldResponses
                    sectionId={section.id}
                    recordId={record.id}
                    sectionType={sectionType}
                  />
                </Box>
                <Divider />
              </Fragment>
            ))
          ) : (
            <Text>No sections available.</Text>
          )}
        </VStack>
      </Box>
    </VStack>
  );

  return (
    <Modal
      title={`${sectionType} Overview`}
      size="lg"
      body={modalBody}
      isOpen={isOpen}
      onClose={onClose}
      actionBtnCaption="Download"
      actionBtnLeftIcon={<Icon as={MdDownload} />}
      onAction={handleDownloadPdf}
      contentMaxW="80vw"
      contentMaxH="90vh"
      bodyMaxH="70vh"
      bodyOverflowY="auto"
    />
  );
};

const Details = ({ title, project, owner }) => (
  <VStack
    align="start"
    fontSize="sm"
    spacing={2}
    p={4}
    borderWidth={1}
    borderRadius={8}
    w="full"
  >
    <Text>
      <strong>Title:</strong> {title}
    </Text>
    <Text>
      <strong>Project:</strong> {project}
    </Text>
    <HStack>
      <Avatar name={owner} size="xs" />
      <Text
        fontSize={{ base: "xs", md: "sm" }}
        fontWeight="light"
        color="gray.700"
      >
        {owner}
      </Text>
    </HStack>
  </VStack>
);

const SectionFieldResponses = ({ sectionId, recordId, sectionType }) => {
  const { data: sectionForm } = useSectionTypeSectionForm[sectionType](
    recordId,
    sectionId
  );

  const values = initialValues(sectionForm.fieldResponses, recordId, sectionId);

  return (
    <VStack align="start" spacing={6}>
      {sectionForm.fieldResponses
        .sort((a, b) => a.sortOrder - b.sortOrder)
        .map(
          (field) =>
            !field.hidden && (
              <Fragment key={field.id}>
                <FieldResponse
                  field={field}
                  sectionId={sectionId}
                  recordId={recordId}
                  ignoreFieldName={sectionType === SECTION_TYPES.Report}
                />
                {field.trigger && (
                  <TriggerFieldResponse
                    field={field}
                    fieldValues={values}
                    sectionFields={sectionForm.fieldResponses}
                    sectionId={sectionId}
                    recordId={recordId}
                    ignoreFieldName={sectionType === SECTION_TYPES.Report}
                  />
                )}
              </Fragment>
            )
        )}
    </VStack>
  );
};

const useSectionTypeSectionForm = {
  [SECTION_TYPES.Plan]: usePlanSection,
  [SECTION_TYPES.Note]: useNoteSection,
  [SECTION_TYPES.Report]: useReportSection,
};
