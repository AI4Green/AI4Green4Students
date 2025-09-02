import {
  Button,
  Divider,
  HStack,
  Icon,
  IconButton,
  Text,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { useSectionsListByProjectType } from "api/section";
import { InlineDraggableListField } from "components/core/forms";
import { BASE_PATH, SimpleBadge } from "components/project-type/canvas/areas";
import { GLOBAL_PARAMETERS, TITLE_ICON_COMPONENTS } from "constants";
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

export const Sections = () => {
  const [searchParams] = useSearchParams();
  const { projectTypeId, sectionTypeId } = useParams();

  const navigate = useNavigate();
  const location = useLocation();

  const [feedback, setFeedback] = useState();
  const [isLoading, setIsLoading] = useState(false);

  const { data: sections, mutate } =
    useSectionsListByProjectType(projectTypeId);

  const { sections: api } = useBackendApi();

  const { t } = useTranslation();
  const toast = useToast();

  const formRef = useRef();

  const isEditing = searchParams.get("action") === "edit";

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

  if (!sectionTypeId) return null;

  return (
    <VStack
      minW="xs"
      p={4}
      align="stretch"
      borderWidth={1}
      borderRadius={7}
      borderColor="purple.100"
    >
      <HStack justify="space-between">
        <SimpleBadge label="Sections" colorScheme="purple" />
        <Actions
          isLoading={isLoading}
          formRef={formRef}
          isEditing={isEditing}
        />
      </HStack>
      <Divider />
      <List isEditing={isEditing} sections={filteredSections} />
      {isEditing && (
        <Formik
          enableReinitialize
          innerRef={formRef}
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
  );
};

const List = ({ isEditing, sections }) => {
  const { projectTypeId, sectionTypeId, sectionId } = useParams();
  const navigate = useNavigate();

  return (
    <VStack spacing={4} align="stretch">
      {!isEditing &&
        sections
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
                      as={TITLE_ICON_COMPONENTS[section.sectionType.name]}
                    />
                  </HStack>
                }
                justifyContent="flex-start"
                w="full"
                variant={Number(sectionId) === section.id ? "solid" : "ghost"}
                size="sm"
                onClick={() => {
                  navigate(
                    `${BASE_PATH}/${projectTypeId}/section-types/${sectionTypeId}/sections/${section.id}`,
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
  );
};

const Actions = ({ isLoading, formRef, isEditing }) => {
  const { projectTypeId, sectionTypeId } = useParams();
  const navigate = useNavigate();

  return (
    <HStack>
      {!isEditing && (
        <IconButton
          size="sm"
          icon={<FaPencilAlt />}
          aria-label="Edit"
          variant="ghost"
          colorScheme="blue"
          onClick={() => {
            navigate(
              `${BASE_PATH}/${projectTypeId}/section-types/${sectionTypeId}/sections?action=edit`,
              {
                replace: true,
              }
            );
          }}
        />
      )}
      {isEditing && (
        <HStack spacing={4}>
          <IconButton
            size="sm"
            icon={<FaSave />}
            aria-label="Save"
            variant="ghost"
            colorScheme="green"
            onClick={() => formRef.current.handleSubmit()}
            fontSize="lg"
            isLoading={isLoading}
          />
          <IconButton
            size="sm"
            fontSize="lg"
            icon={<TbCancel />}
            aria-label="Cancel"
            variant="ghost"
            colorScheme="yellow"
            onClick={() => {
              navigate(
                `${BASE_PATH}/${projectTypeId}/section-types/${sectionTypeId}/sections`,
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
  );
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
