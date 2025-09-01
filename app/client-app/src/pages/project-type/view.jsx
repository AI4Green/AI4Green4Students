import {
  Button,
  Divider,
  HStack,
  Icon,
  IconButton,
  Stack,
  Text,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { useSectionTypesList } from "api/project-type";
import { useSectionsListByProjectType } from "api/section";
import { InlineDraggableListField } from "components/core/forms";
import { FormikInput } from "components/core/forms/formik-input";
import {
  GLOBAL_PARAMETERS,
  SECTION_TYPES,
  TITLE_ICON_COMPONENTS,
} from "constants";
import { useBackendApi } from "contexts";
import { Form, Formik } from "formik";
import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { FaPencilAlt, FaSave } from "react-icons/fa";
import { TbCancel } from "react-icons/tb";
import {
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";
import { array, object, string } from "yup";

export const ProjectTypeView = () => {
  const basePath = "/admin/project-type-management";

  const [searchParams] = useSearchParams();
  const { projectTypeId, sectionTypeId, sectionId } = useParams();

  const action = searchParams.get("action");
  const isEditing = action === "edit";

  const navigate = useNavigate();
  const location = useLocation();

  const { data: sectionTypes } = useSectionTypesList();
  const { data: sections, mutate } =
    useSectionsListByProjectType(projectTypeId);

  const { sections: api } = useBackendApi();

  const { t } = useTranslation();

  const toast = useToast();

  const sectionsFormRef = useRef();

  const [feedback, setFeedback] = useState();
  const [isLoading, setIsLoading] = useState(false);

  const filteredSections = sections?.filter(
    (section) => section.sectionType.id === Number(sectionTypeId)
  );

  const handleSectionsSubmit = async ({ sections }) => {
    setIsLoading(true);
    const model = {
      projectTypeId: Number(projectTypeId),
      sectionTypeId: Number(sectionTypeId),
      sections: sections.map((section) => ({
        id: section.id.startsWith("temp") ? null : Number(section.id),
        name: section.content,
        sortOrder: section.order,
      })),
    };

    try {
      await api.save(model);
      setIsLoading(false);

      toast({
        title: "Sections saved",
        status: "success",
        duration: GLOBAL_PARAMETERS.ToastDuration,
        isClosable: true,
        position: "top",
      });

      navigate(location.pathname, { replace: true });
      await mutate();
    } catch (e) {
      switch (e?.response?.status) {
        case 400: {
          setFeedback({
            status: "error",
            message: t("feedback.error_400"),
          });
          break;
        }
        default: {
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
          break;
        }
      }
      toast({
        title: feedback.message,
        status: "error",
        duration: GLOBAL_PARAMETERS.ToastDuration,
        isClosable: true,
        position: "top",
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Stack align="center" minW="full" pb={8}>
      <HStack
        align="start"
        spacing={8}
        w={{ base: "full", xl: "90%", "2xl": "70%" }}
      >
        <VStack
          p={4}
          spacing={4}
          align="start"
          borderWidth={1}
          borderRadius={7}
        >
          <Text fontSize="md" fontWeight="semibold">
            Areas
          </Text>
          <Divider />
          {sectionTypes.map((sectionType) => (
            <Button
              key={sectionType.id}
              justifyContent="flex-start"
              w="full"
              leftIcon={<Icon as={TITLE_ICON_COMPONENTS[sectionType.name]} />}
              variant={
                Number(sectionTypeId) === sectionType.id ? "solid" : "ghost"
              }
              size="sm"
              onClick={() => {
                navigate(
                  `${basePath}/${projectTypeId}/section-types/${sectionType.id}/sections`,
                  {
                    replace: true,
                  }
                );
              }}
            >
              {sectionTypeLabels[sectionType.name]}
            </Button>
          ))}
        </VStack>

        {sectionTypeId && (
          <VStack
            minW="sm"
            p={4}
            align="stretch"
            borderWidth={1}
            borderRadius={7}
          >
            <HStack justify="space-between">
              <Text fontSize="md" fontWeight="semibold">
                Sections
              </Text>
              <HStack>
                {!isEditing && (
                  <IconButton
                    icon={<FaPencilAlt />}
                    aria-label="Edit"
                    variant="ghost"
                    colorScheme="blue"
                    onClick={() => {
                      navigate(
                        `${basePath}/${projectTypeId}/section-types/${sectionTypeId}/sections?action=edit`,
                        {
                          replace: true,
                        }
                      );
                    }}
                  />
                )}
                {isEditing && (
                  <HStack spacing={1}>
                    <IconButton
                      icon={<FaSave />}
                      aria-label="Save"
                      variant="ghost"
                      colorScheme="green"
                      onClick={() => sectionsFormRef.current.handleSubmit()}
                      fontSize="lg"
                      isLoading={isLoading}
                    />
                    <IconButton
                      fontSize="lg"
                      icon={<TbCancel />}
                      aria-label="Cancel"
                      variant="ghost"
                      colorScheme="red"
                      onClick={() => {
                        navigate(
                          `${basePath}/${projectTypeId}/section-types/${sectionTypeId}/sections`,
                          {
                            replace: true,
                          }
                        );
                      }}
                      isLoading={isLoading}
                    />
                  </HStack>
                )}
              </HStack>
            </HStack>
            <Divider />

            <VStack spacing={8} align="stretch">
              {!isEditing &&
                filteredSections
                  .sort((a, b) => a.sortOrder - b.sortOrder)
                  .map((section) => (
                    <HStack key={section.id} spacing={2}>
                      <Button
                        leftIcon={
                          <HStack spacing={4}>
                            <Text fontSize="xs" fontWeight="light">
                              {section.sortOrder}.
                            </Text>
                            <Icon
                              as={
                                TITLE_ICON_COMPONENTS[section.sectionType.name]
                              }
                            />
                          </HStack>
                        }
                        justifyContent="flex-start"
                        w="full"
                        variant={
                          Number(sectionId) === section.id ? "solid" : "ghost"
                        }
                        size="md"
                        onClick={() => {
                          navigate(
                            `${basePath}/${projectTypeId}/section-types/${sectionTypeId}/sections/${section.id}/fields`,
                            {
                              replace: true,
                            }
                          );
                        }}
                      >
                        {section.name}
                      </Button>
                    </HStack>
                  ))}
            </VStack>

            {isEditing && (
              <Formik
                enableReinitialize
                innerRef={sectionsFormRef}
                initialValues={{
                  sections:
                    filteredSections?.map((section) => ({
                      id: section.id.toString(),
                      content: section.name,
                      order: section.sortOrder,
                    })) || [],
                }}
                onSubmit={handleSectionsSubmit}
                validationSchema={validationSchema}
              >
                <Form>
                  <InlineDraggableListField
                    name="sections"
                    addLabel="Add new section"
                  />
                </Form>
              </Formik>
            )}
          </VStack>
        )}

        <VStack
          p={4}
          spacing={4}
          align="stretch"
          borderWidth={1}
          borderRadius={7}
        >
          <Text> Fields</Text>
        </VStack>
      </HStack>
    </Stack>
  );
};

const sectionTypeLabels = {
  [SECTION_TYPES.LiteratureReview]: "Literature Review",
  [SECTION_TYPES.Plan]: "Plan",
  [SECTION_TYPES.Note]: "Note",
  [SECTION_TYPES.Report]: "Report",
  [SECTION_TYPES.ProjectGroup]: "Project Group Activities",
};

const validationSchema = object().shape({
  sections: array().of(
    object().shape({
      content: string()
        .required("Section name is required")
        .test("unique", "Duplicate section name", function (value) {
          if (!value) return true;

          const allSections = this.parent?.parent;

          if (!allSections || !Array.isArray(allSections)) return true;

          const normalized = value.trim().toLowerCase();
          const duplicateCount = allSections.filter(
            (section) => section.content?.trim().toLowerCase() === normalized
          ).length;

          return duplicateCount <= 1;
        }),
    })
  ),
});
