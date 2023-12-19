import React, { useEffect, useState } from "react";
import Autocomplete from "@mui/material/Autocomplete";
import TextField from "@mui/material/TextField";

const MultiplySelectSubCategory = ({
  subCategories,
  selectedValues,
  selectedSubCategories,
  selectedCategory,
}) => {
  const [prevCategory, setPrevCategory] = useState(selectedCategory);

  useEffect(() => {
    // Check if the category has changed
    if (selectedCategory !== prevCategory) {
      // Clear selected sub-categories
      selectedSubCategories([]);
      // Update the previous category
      setPrevCategory(selectedCategory);
    }

    // console.log(subCategories);
    // console.log(subCategories);
  }, [subCategories, selectedValues, selectedSubCategories]);

  return (
    <Autocomplete
      size="small"
      sx={{ marginTop: 2 }}
      disabled={subCategories.length === 0 && selectedValues.length === 0}
      multiple
      id="sub-category-selector"
      options={subCategories}
      value={selectedValues}
      onChange={(event, newValue) => selectedSubCategories(newValue)}
      isOptionEqualToValue={(option, value) =>
        option.id === value.id || option.nameKz === value.id
      }
      getOptionLabel={(option) => option.nameKz}
      renderInput={(params) => (
        <TextField
          {...params}
          label="Құжатның ішкі категориясын таңдаңыз"
          variant="outlined"
        />
      )}
      key={(option) => option.id}
    />
  );
};

export default MultiplySelectSubCategory;
