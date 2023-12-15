import {
  Input,
  VStack,
  Text,
  Button,
  Alert,
  AlertIcon,
  HStack,
  Tag,
  TagLeftIcon,
  TagLabel,
  TagCloseButton,
  useDisclosure,
  Link,
  FormControl,
  FormLabel,
} from "@chakra-ui/react";
import { useField } from "formik";
import { FaCloudUploadAlt } from "react-icons/fa";
import { useState, useEffect } from "react";
import { MdCheckCircle } from "react-icons/md";
import { ErrorAlert } from "components/ErrorAlert";
import { useRef } from "react";
import { BasicModal } from "components/BasicModal";

const InfoAlert = ({ accept }) => {
  return (
    <Alert borderRadius={7} variant="left-accent">
      <AlertIcon />
      <VStack width="100%" align="start">
        <Text>You can attach a paper-full text in a supported format:</Text>
        <HStack>
          {accept?.map((extension, index) => (
            <Tag key={index} variant="subtle" colorScheme="green">
              <TagLeftIcon boxSize="12px" as={MdCheckCircle} />
              <TagLabel>{extension}</TagLabel>
            </Tag>
          ))}
        </HStack>
      </VStack>
    </Alert>
  );
};

const modal = (
  <VStack align="flex-start">
    <Text fontSize="md" color="gray.600">
      You have chosen to remove the uploaded file.
    </Text>
    <Text fontSize="sm">
      By proceeding and saving the form, your previously uploaded file will be
      permanently deleted and cannot be recovered.
    </Text>
  </VStack>
);

export const FileUploadField = ({
  name, // name of the field that actually holds the file
  title,
  isFilePresentName, // name of the field that holds a bool which represents if file is present
  accept, // array of accepted file extensions
  existingFile, // an existing filename
  downloadLink = "#",
  isRequired,
  isDisabled,
}) => {
  const DeleteExistingState = useDisclosure();
  const [fieldFileUpload, metaFileUpload, helpersFileUpload] = useField(name);
  const [fieldIsFilePresent, metaIsFilePresent, helpersIsFilePresent] =
    useField(isFilePresentName); // bool that represents if file is present

  const [fileName, setFileName] = useState();
  const [uploaded, setUploaded] = useState(false);
  const [fileError, setFileError] = useState(false);
  const [removeExisting, setRemoveExisting] = useState(false);

  const fileInputRef = useRef();

  const handleBtnClick = () => fileInputRef.current.click(); // trigger file selector dialog box

  const handleChange = ({ target: { files } }) => {
    const fileExt = files[0].name
      .slice(files[0].name.lastIndexOf("."))
      .toLowerCase();
    const isAccepted = accept.some((ext) => ext.toLowerCase() === fileExt);

    if (!isAccepted) {
      setFileError(true);
      helpersFileUpload.setValue(null);
      return;
    }

    setFileError(false);
    setUploaded(true);
    helpersFileUpload.setValue(files[0]);
    helpersIsFilePresent.setValue(true);
    setFileName(files[0].name);
  };

  const handleRemove = () => {
    setUploaded(false);
    helpersFileUpload.setValue(null);
    helpersIsFilePresent.setValue(
      existingFile
        ? !removeExisting // true if there is an existing file and existing file not removed
        : false
    );
    setFileName(null);
  };

  const handleRemoveExisting = () => setRemoveExisting(true);

  useEffect(
    () => existingFile && helpersIsFilePresent.setValue(!removeExisting),
    [removeExisting]
  );

  const UploadSection = () => (
    <>
      {fileError && (
        <ErrorAlert status="error" message="Please upload a valid file" />
      )}
      {uploaded && (
        <ErrorAlert status="success" message="File successfully added!" />
      )}
      <HStack w="100%">
        <Button
          colorScheme="blue"
          variant="outline"
          size="sm"
          leftIcon={<FaCloudUploadAlt />}
          onClick={handleBtnClick}
        >
          Upload
        </Button>
        <Input
          name={name}
          type="file"
          display="none"
          accept={accept}
          onChange={handleChange}
          mt="10px"
          ref={fileInputRef}
        />

        {fileName && (
          <Tag variant="outline">
            <TagLabel>{fileName}</TagLabel>
            <TagCloseButton onClick={handleRemove} />
          </Tag>
        )}
      </HStack>
    </>
  );

  const RemoveExisitngButton = () =>
    !fileName &&
    existingFile &&
    !removeExisting && (
      <>
        <TagCloseButton onClick={DeleteExistingState.onOpen} />
        {DeleteExistingState.isOpen && (
          <BasicModal
            body={modal}
            title="🚫 File removal warning"
            actionBtnCaption="Continue"
            actionBtnColorScheme="orange"
            onAction={handleRemoveExisting}
            isOpen={DeleteExistingState.isOpen}
            onClose={DeleteExistingState.onClose}
          />
        )}
      </>
    );

  return (
    <FormControl id={fieldFileUpload.name} isRequired={isRequired}>
      <VStack align="start" w="100%" spacing={2}>
        <FormLabel>
          <Text as="b">{title}</Text>
        </FormLabel>
        <InfoAlert accept={accept} />

        {!isDisabled && <UploadSection />}

        {!fileName && existingFile && !removeExisting && (
          <Tag colorScheme="green">
            <TagLabel>
              <Link href={downloadLink}>{existingFile}</Link>
            </TagLabel>

            {!isDisabled && <RemoveExisitngButton />}
          </Tag>
        )}
      </VStack>
    </FormControl>
  );
};
