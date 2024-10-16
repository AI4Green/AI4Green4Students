import {
  VStack,
  Text,
  Alert,
  AlertIcon,
  HStack,
  Tag,
  TagLeftIcon,
  TagLabel,
  FormControl,
  FormLabel,
  AlertDescription,
  Flex,
  Input,
} from "@chakra-ui/react";
import { useField } from "formik";
import { useEffect, useState } from "react";
import { MdCheckCircle } from "react-icons/md";
import { UploadImage, ImageForUpload } from "./UploadImage";
import { UploadedImage } from "./UploadedImage";
import { FormHelpError } from "../FormHelpError";

/**
 * This is a formik field component for uploading images with captions.
 *
 * The structure of the field value is an array of objects:
 * - file: the file object
 * - name: the file name
 * - isNew: a flag to indicate if the file is newly uploaded
 * - isMarkedForDeletion: a flag to indicate if the file is marked for deletion (only applied to existing files)
 * - caption: the caption of the image
 *
 */

export const ImageUploadField = ({
  name,
  title,
  accept,
  isRequired,
  isDisabled,
}) => {
  const [field, meta, helpers] = useField(name);
  const [fileUploadError, setFileUploadError] = useState([]);

  useEffect(() => {
    /**
     * For handling caption error set by the validation schema.
     * This is necessary to as the error come as an array of objects as field value is an array of object.
     * Each object in the array corresponds to the error of the field value at the same index.
     * Thus, we are mapping through the field value and adding the error object.
     * It is then used by the component to display the error message correctly.
     * Similarly, when there is no error, we remove the error key from the file object.
     */
    const updated = field.value.map((file, i) => {
      if (Array.isArray(meta.error)) {
        return { ...file, error: { caption: meta.error[i]?.caption } };
      }
      const { error, ...other } = file; // remove error key when there is no error
      return other;
    });
    helpers.setValue(updated);
  }, [meta.error]);

  return (
    <FormControl
      id={field.name}
      isRequired={isRequired}
      isInvalid={meta.error && meta.touched}
    >
      <VStack align="start" w="100%" spacing={2}>
        <FormLabel>{title}</FormLabel>

        {!isDisabled && (
          <>
            <Info accept={accept} />
            {fileUploadError &&
              fileUploadError.map((e, i) => (
                <ImageForUploadAlert
                  key={i}
                  status="error"
                  message={e.message}
                />
              ))}

            <VStack w="full" align="flex-start">
              <UploadImage
                accept={accept}
                setFileUploadError={setFileUploadError}
                values={field.value}
                helpers={helpers}
              />

              <VStack align="start" w="100%">
                {field.value
                  ?.filter((image) => image.isNew)
                  .map((item) => (
                    <ImageForUpload
                      key={item.name}
                      setFileUploadError={setFileUploadError}
                      fileUploadError={fileUploadError}
                      image={item}
                      values={field.value}
                      helpers={helpers}
                    />
                  ))}
              </VStack>
            </VStack>
          </>
        )}
        <Flex wrap="wrap" gap={2}>
          {field.value
            ?.filter((file) => !file.isMarkedForDeletion && !file.isNew)
            .map((item) => (
              <UploadedImage
                key={item.name}
                image={item}
                values={field.value}
                helpers={helpers}
              />
            ))}
        </Flex>
      </VStack>
      <FormHelpError
        isInvalid={meta.touched && meta.error && !Array.isArray(meta.error)} // only show if not an array
        error={meta.error}
        collapseEmpty
        replaceHelpWithError
      />
    </FormControl>
  );
};

const Info = ({ accept }) => {
  return (
    <Alert borderRadius={7} variant="left-accent" colorScheme="gray" py={2}>
      <AlertIcon />
      <HStack align="start">
        <Text fontSize="xs">Supported format</Text>
        <HStack>
          {accept?.map((extension, index) => (
            <Tag key={index} variant="subtle" colorScheme="green">
              <TagLeftIcon boxSize="12px" as={MdCheckCircle} />
              <TagLabel>{extension}</TagLabel>
            </Tag>
          ))}
        </HStack>
      </HStack>
    </Alert>
  );
};

// Displays whether or not file has been added to the list of files to be uploaded
const ImageForUploadAlert = ({ status, message }) => {
  return (
    <Alert status={status} py={2}>
      <AlertIcon boxSize={4} />
      <AlertDescription>
        <Text fontSize="xs">{message}</Text>
      </AlertDescription>
    </Alert>
  );
};

/**
 * Input field for inputting image caption.
 */
export const CaptionInput = ({ image, values, helpers }) => {
  // Handle the image caption input changes
  const handleCaptionInputChange = (e) => {
    const updatedImages = [...values];
    const fileIndex = updatedImages.findIndex(
      (item) => item.name === image.name
    );
    updatedImages[fileIndex] = { ...image, caption: e.target.value };
    helpers.setValue(updatedImages);
  };
  return (
    <FormControl isInvalid={image.error?.caption}>
      <Input
        mt={4}
        px={2}
        variant="flushed"
        placeholder="Add image caption"
        size="xs"
        onChange={handleCaptionInputChange}
        value={image?.caption}
        color="gray.500"
      />
      <FormHelpError
        isInvalid={image.error?.caption}
        error={image.error?.caption}
        collapseEmpty
        replaceHelpWithError
      />
    </FormControl>
  );
};
