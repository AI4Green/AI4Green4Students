import { useCallback, useState } from "react";

export const useModalState = (location, navigate, formRef = null) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [feedback, setFeedback] = useState();

  const handleReset = useCallback(() => {
    setFeedback();
    setIsLoading(false);
    setIsModalOpen(false);
    formRef?.current?.resetForm();
    navigate(location.pathname, { replace: true });
  }, [location.pathname, navigate, formRef]);

  return {
    isModalOpen,
    setIsModalOpen,
    isLoading,
    setIsLoading,
    feedback,
    setFeedback,
    handleReset,
  };
};
