import {
  FaLeaf,
  FaInfoCircle,
  FaCalculator,
  FaBook,
  FaGithub,
} from "react-icons/fa";
import { LuLoaderPinwheel } from "react-icons/lu";

export const navbarItems = [
  {
    label: "About",
    href: "/about",
    icon: FaInfoCircle,
  },
  {
    label: "Green Chemistry",
    href: "/greenchemistry",
    icon: FaLeaf,
  },
  {
    label: "Sustainability Metrics",
    href: "/metrics",
    icon: FaCalculator,
  },
  {
    label: "Reaction Predictions",
    href: "/reaction-predictions",
    icon: LuLoaderPinwheel,
  },
  {
    label: "Documentation",
    href: "/documentation",
    icon: FaBook,
  },
  {
    label: "GitHub",
    href: "https://github.com/AI4Green/AI4Green4Students",
    icon: FaGithub,
  },
];
