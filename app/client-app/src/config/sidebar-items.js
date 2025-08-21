import {
  REGISTRATION_RULES_PERMISSIONS,
  USERMANAGEMENT_PERMISSIONS,
} from "constants";
import { FaPencilRuler, FaUserCog } from "react-icons/fa";

export const getSidebarItems = (t) => [
  {
    label: t("adminMenu.menuList.userManagement"),
    href: "/admin/usermanagement",
    icon: FaUserCog,
    permission: USERMANAGEMENT_PERMISSIONS,
  },
  {
    label: t("adminMenu.menuList.registrationRule"),
    href: "/admin/registrationrule",
    icon: FaPencilRuler,
    permission: REGISTRATION_RULES_PERMISSIONS,
  },
];
