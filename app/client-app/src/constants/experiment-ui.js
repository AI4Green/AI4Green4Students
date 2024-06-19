/**
 * Contains constants for the UI of the experiment.
 */

import {
  FaBook,
  FaTasks,
  FaChartBar,
  FaPencilAlt,
  FaSearch,
  FaClock,
  FaCheckCircle,
} from "react-icons/fa";
import { SECTION_TYPES } from "./section-types";
import { STAGES } from "./stages";

export const TITLE_ICON_COMPONENTS = {
  [SECTION_TYPES.LiteratureReview]: FaBook,
  [SECTION_TYPES.Plan]: FaTasks,
  [SECTION_TYPES.Report]: FaChartBar,
};

export const STATUS_ICON_COMPONENTS = {
  [STAGES.Draft]: { icon: FaPencilAlt, color: "gray" },
  [STAGES.InReview]: { icon: FaSearch, color: "purple" },
  [STAGES.AwaitingChanges]: { icon: FaClock, color: "orange" },
  [STAGES.Approved]: { icon: FaCheckCircle, color: "green" },
};
