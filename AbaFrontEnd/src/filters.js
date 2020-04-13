/* eslint no-useless-escape:0 */

export function phone(value) {
  return value
    ? value
        .toString()
        .replace(/[^0-9]/g, "")
        .replace(/(\d{3})(\d{3})(\d{4})/, "($1) $2-$3")
    : "N/A";
}

export function socialSecurity(value) {
  return value ? value.toString().replace(/^(\d{3})\s?\-?\s?(\d{2})\s?\-?\s?(\d{3})/, "$1-$2-$3") : "N/A";
}
